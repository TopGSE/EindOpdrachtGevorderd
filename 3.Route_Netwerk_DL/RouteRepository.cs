using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2.Route_Netwerk_BL.Exceptions;
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
            string query = "SELECT Id, Naam FROM Routes";
            List<Route> routes = new();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string naam = reader.GetString(1);
                        routes.Add(new Route(id, naam));
                    }
                }
            }

            foreach (var route in routes)
            {
                route.Punten = HaalPuntenOp(route.Id);
            }

            return routes;
        }

        public List<NetworkPoint> HaalPuntenOp(int id)
        {
            string query = @"SELECT np.Id, np.X, np.Y, rp.IsStopPlaats FROM RoutePoints rp JOIN NetworkPoints rp.NetworkPointId = np.IdWHERE rp.RouteId = @RouteId";

            List<NetworkPoint> punten = new();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RouteId", id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int puntId = reader.GetInt32(0);
                        double x = reader.GetDouble(1);
                        double y = reader.GetDouble(2);
                        bool isStopPlaats = reader.GetBoolean(3);

                        punten.Add(new NetworkPoint(puntId, x, y) { IsStopPlaats = isStopPlaats });
                    }
                }
            }
            return punten;
        }

        public void VoegRouteToe(string naam, List<NetworkPoint> punten)
        {
            string insertRouteQuery = "INSERT INTO Routes (Naam) OUTPUT INSERTED.Id VALUES (@Naam)";
            string insertPointQuery = "INSERT INTO RoutePoints (RouteId, NetworkPointId, IsStopPlaats) VALUES (@RouteId, @PointId, @IsStop)";
            string insertSegmentQuery = "INSERT INTO RouteSegments (RouteId, SegmentId, Volgorde) VALUES (@RouteId, @SegmentId, @Volgorde)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    int routeId;

                    using (SqlCommand cmd = new SqlCommand(insertRouteQuery, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Naam", naam);
                        routeId = (int)cmd.ExecuteScalar();
                    }

                    for (int i = 0; i < punten.Count; i++)
                    {
                        var punt = punten[i];

                        using (SqlCommand cmd = new SqlCommand(insertPointQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@RouteId", routeId);
                            cmd.Parameters.AddWithValue("@PointId", punt.Id);
                            cmd.Parameters.AddWithValue("@IsStop", punt.IsStopPlaats);
                            cmd.ExecuteNonQuery();
                        }

                        if (i < punten.Count - 1)
                        {
                            int segmentId = HaalSegmentIdOp(punten[i].Id, punten[i + 1].Id, conn, tx);

                            using (SqlCommand cmd = new SqlCommand(insertSegmentQuery, conn, tx))
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
        }

        public int HaalSegmentIdOp(int vanId, int naarId, SqlConnection conn, SqlTransaction tx)
        {
            string query = @"SELECT Id FROM Segments WHERE (StartPointId = @From AND EndPointId = @To) OR (StartPointId = @To AND EndPointId = @From)";

            using (SqlCommand cmd = new SqlCommand(query, conn, tx))
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
            string routeQuery = "SELECT Naam FROM Routes WHERE Id = @Id";
            string puntenQuery = @"SELECT rp.NetworkPointId, np.X, np.Y, rp.IsStopPlaats FROM RoutePoints rp JOIN NetworkPoints np ON rp.NetworkPointId = np.Id WHERE rp.RouteId = @Id ORDER BY rp.RouteId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Route route = null;
                List<NetworkPoint> punten = new();

                using (SqlCommand cmd = new SqlCommand(routeQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string naam = reader.GetString(0);
                            route = new Route(id, naam, punten);
                        }
                    }
                }

                if (route == null)
                    throw new Exception("Route not found");

                using (SqlCommand cmd = new SqlCommand(puntenQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pointId = reader.GetInt32(0);
                            double x = reader.GetDouble(1);
                            double y = reader.GetDouble(2);
                            bool isStop = reader.GetBoolean(3);

                            punten.Add(new NetworkPoint(pointId, x, y) { IsStopPlaats = isStop });
                        }
                    }
                }

                return route;
            }
        }

        public void UpdateRoute(Route loadedRoute)
        {
            string updateRouteQuery = "UPDATE Routes SET Naam = @Naam WHERE Id = @Id";
            string deletePointsQuery = "DELETE FROM RoutePoints WHERE RouteId = @Id";
            string deleteSegmentsQuery = "DELETE FROM RouteSegments WHERE RouteId = @Id";
            string insertPointQuery = "INSERT INTO RoutePoints (RouteId, NetworkPointId, IsStopPlaats) VALUES (@RouteId, @PointId, @IsStop)";
            string insertSegmentQuery = "INSERT INTO RouteSegments (RouteId, SegmentId, Volgorde) VALUES (@RouteId, @SegmentId, @Volgorde)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(updateRouteQuery, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Naam", loadedRoute.Naam);
                        cmd.Parameters.AddWithValue("@Id", loadedRoute.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(deletePointsQuery, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Id", loadedRoute.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(deleteSegmentsQuery, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Id", loadedRoute.Id);
                        cmd.ExecuteNonQuery();
                    }

                    for (int i = 0; i < loadedRoute.Punten.Count; i++)
                    {
                        var punt = loadedRoute.Punten[i];

                        using (SqlCommand cmd = new SqlCommand(insertPointQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@RouteId", loadedRoute.Id);
                            cmd.Parameters.AddWithValue("@PointId", punt.Id);
                            cmd.Parameters.AddWithValue("@IsStop", punt.IsStopPlaats);
                            cmd.ExecuteNonQuery();
                        }

                        if (i < loadedRoute.Punten.Count - 1)
                        {
                            int segmentId = HaalSegmentIdOp(loadedRoute.Punten[i].Id, loadedRoute.Punten[i + 1].Id, conn, tx);

                            using (SqlCommand cmd = new SqlCommand(insertSegmentQuery, conn, tx))
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

        public void DeleteRoute(int id)
        {
            string deletePointsQuery = "DELETE FROM RoutePoints WHERE RouteId = @Id";
            string deleteSegmentsQuery = "DELETE FROM RouteSegments WHERE RouteId = @Id";
            string deleteRouteQuery = "DELETE FROM Routes WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand deletePointsCmd = conn.CreateCommand())
            using (SqlCommand deleteSegmentsCmd = conn.CreateCommand())
            using (SqlCommand deleteRouteCmd = conn.CreateCommand())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                deletePointsCmd.Transaction = transaction;
                deleteSegmentsCmd.Transaction = transaction;
                deleteRouteCmd.Transaction = transaction;

                try
                {
                    // Verwijder RoutePoints
                    deletePointsCmd.CommandText = deletePointsQuery;
                    deletePointsCmd.Parameters.AddWithValue("@Id", id);
                    deletePointsCmd.ExecuteNonQuery();
                    deletePointsCmd.Parameters.Clear();

                    // Verwijder RouteSegments
                    deleteSegmentsCmd.CommandText = deleteSegmentsQuery;
                    deleteSegmentsCmd.Parameters.AddWithValue("@Id", id);
                    deleteSegmentsCmd.ExecuteNonQuery();
                    deleteSegmentsCmd.Parameters.Clear();

                    // Verwijder de Route zelf
                    deleteRouteCmd.CommandText = deleteRouteQuery;
                    deleteRouteCmd.Parameters.AddWithValue("@Id", id);
                    deleteRouteCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (RouteException ex)
                {
                    transaction.Rollback();
                    throw new RouteException("Fout bij het verwijderen van route: " + ex.Message, ex);
                }
            }
        }

    }
}
