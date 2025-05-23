using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;


namespace _3.Route_Netwerk_DL
{
    public class RouteRepository : IRouteRepository
    {
        private List<Route> _routes = new();
        private NetwerkRepository _netwerkrepo;
        private string connectionString = @"Data Source=SOLOS-LAPTOP\SQLEXPRESS;Initial Catalog=RouteNetwerkDB;Integrated Security=True;Trust Server Certificate=True";

        public List<Route> GetAllRoutes()
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT Id, Naam FROM Routes", conn))
            {
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string naam = reader.GetString(1);
                    _routes.Add(new Route(id, naam));
                }
            }
            foreach (var route in _routes)
            {
                route.Punten = HaalPuntenOp(route.Id);
            }
            return _routes;

        }
        public List<NetworkPoint> HaalPuntenOp(int id)
        {
            List<NetworkPoint> punten = new();
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT np.Id, np.X, np.Y, rp.IsStopPlaats
                FROM RoutePoints rp
                JOIN NetworkPoints np ON rp.NetworkPointId = np.Id
                WHERE rp.RouteId = @RouteId", conn))
            {
                cmd.Parameters.AddWithValue("@RouteId", id);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int puntId = reader.GetInt32(0);
                    double x = reader.GetDouble(1);
                    double y = reader.GetDouble(2);
                    bool isStopPlaats = reader.GetBoolean(3);
                    punten.Add(new NetworkPoint(puntId, x, y) { IsStopPlaats = isStopPlaats });
                }
            }
            return punten;

        }
        public void VoegRouteToe(string naam, List<NetworkPoint> punten)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tx = conn.BeginTransaction();

            try
            {
                int routeId;

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Routes (Naam) OUTPUT INSERTED.Id VALUES (@Naam)", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@Naam", naam);
                    routeId = (int)cmd.ExecuteScalar();
                }

                for (int i = 0; i < punten.Count; i++)
                {
                    var punt = punten[i];

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO RoutePoints (RouteId, NetworkPointId, IsStopPlaats) VALUES (@RouteId, @PointId, @IsStop)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@RouteId", routeId);
                        cmd.Parameters.AddWithValue("@PointId", punt.Id);
                        cmd.Parameters.AddWithValue("@IsStop", punt.IsStopPlaats);
                        cmd.ExecuteNonQuery();
                    }

                    if (i < punten.Count - 1)
                    {
                        int segmentId = HaalSegmentIdOp(punten[i].Id, punten[i + 1].Id, conn, tx);

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO RouteSegments (RouteId, SegmentId, Volgorde) VALUES (@RouteId, @SegmentId, @Volgorde)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@RouteId", routeId);
                            cmd.Parameters.AddWithValue("@SegmentId", segmentId);
                            cmd.Parameters.AddWithValue("@Volgorde", i);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        public int HaalSegmentIdOp(int vanId, int naarId, SqlConnection conn, SqlTransaction tx)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT Id FROM Segments
                WHERE (StartPointId = @From AND EndPointId = @To)
                   OR (StartPointId = @To AND EndPointId = @From)", conn, tx))
            {
                cmd.Parameters.AddWithValue("@From", vanId);
                cmd.Parameters.AddWithValue("@To", naarId);

                object result = cmd.ExecuteScalar();
                if (result == null)
                    throw new Exception($"Geen segment tussen punt {vanId} en {naarId}");

                return (int)result;
            }
        }
        public Route GetRouteById(int id)
        {
            Route route = null;
            List<NetworkPoint> punten = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Haal de route-naam op
            using (SqlCommand cmd = new SqlCommand("SELECT Naam FROM Routes WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string naam = reader.GetString(0);
                    route = new Route(id, naam, punten);
                }

                reader.Close();
            }

            if (route == null)
            {
                throw new Exception("Route not found");
            }

            // Haal de bijhorende netwerkpunten op
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT rp.NetworkPointId, np.X, np.Y, rp.IsStopPlaats 
                FROM RoutePoints rp
                JOIN NetworkPoints np ON rp.NetworkPointId = np.Id
                WHERE rp.RouteId = @Id
                ORDER BY rp.RouteId", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int pointId = reader.GetInt32(0);
                    double x = reader.GetDouble(1);  // X is kolom 1
                    double y = reader.GetDouble(2);  // Y is kolom 2
                    bool isStop = reader.GetBoolean(3); // IsStopPlaats is kolom 3

                    punten.Add(new NetworkPoint(pointId, x, y)
                    {
                        IsStopPlaats = isStop
                    });
                }

                reader.Close();
            }

            return route;
        }

        public void UpdateRoute(Route loadedRoute)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tx = conn.BeginTransaction();
            try
            {
                // Update de route naam
                using (SqlCommand cmd = new SqlCommand("UPDATE Routes SET Naam = @Naam WHERE Id = @Id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@Naam", loadedRoute.Naam);
                    cmd.Parameters.AddWithValue("@Id", loadedRoute.Id);
                    cmd.ExecuteNonQuery();
                }
                // Verwijder bestaande punten en segmenten
                using (SqlCommand cmd = new SqlCommand("DELETE FROM RoutePoints WHERE RouteId = @Id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@Id", loadedRoute.Id);
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand("DELETE FROM RouteSegments WHERE RouteId = @Id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@Id", loadedRoute.Id);
                    cmd.ExecuteNonQuery();
                }
                // Voeg nieuwe punten en segmenten toe
                for (int i = 0; i < loadedRoute.Punten.Count; i++)
                {
                    var punt = loadedRoute.Punten[i];
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO RoutePoints (RouteId, NetworkPointId, IsStopPlaats) VALUES (@RouteId, @PointId, @IsStop)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@RouteId", loadedRoute.Id);
                        cmd.Parameters.AddWithValue("@PointId", punt.Id);
                        cmd.Parameters.AddWithValue("@IsStop", punt.IsStopPlaats);
                        cmd.ExecuteNonQuery();
                    }
                    if (i < loadedRoute.Punten.Count - 1)
                    {
                        int segmentId = HaalSegmentIdOp(loadedRoute.Punten[i].Id, loadedRoute.Punten[i + 1].Id, conn, tx);
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO RouteSegments (RouteId, SegmentId, Volgorde) VALUES (@RouteId, @SegmentId, @Volgorde)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@RouteId", loadedRoute.Id);
                            cmd.Parameters.AddWithValue("@SegmentId", segmentId);
                            cmd.Parameters.AddWithValue("@Volgorde", i);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw new Exception("Fout bij het updaten van de route: " + ex.Message);
            }
        }
    }
}
