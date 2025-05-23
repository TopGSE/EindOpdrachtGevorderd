using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _3.Route_Netwerk_DL 

{
    public class NetwerkRepository : INetwerkRepository
    {
        private string connectionString = @"Data Source=SOLOS-LAPTOP\SQLEXPRESS;Initial Catalog=RouteNetwerkDB;Integrated Security=True;Trust Server Certificate=True";
        private List<NetworkPoint> _networkPoints = new();
        private List<(int Point1Id, int Point2Id)> _connections = new();
        private List<Facility> _facilities = new();

        public List<NetworkPoint> GetNetworkPoints()
        {
            _networkPoints.Clear();
            string query = "SELECT * FROM NetworkPoints";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = query;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var networkPoint = new NetworkPoint(
                            reader.GetInt32(0),
                            reader.GetDouble(1),
                            reader.GetDouble(2)
                        );
                        _networkPoints.Add(networkPoint);
                        networkPoint.Facilities = GeefFaciliteitenVoorPoint(networkPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving network points: {ex.Message}");
                }
            }
            return _networkPoints;
        }
        public void VoegFaciliteitToe(Facility facility)
        {
            string query = "INSERT INTO Facilities (Naam) OUTPUT INSERTED.Id VALUES (@Naam)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@Naam", facility.Naam);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        facility.Id = newId;
                    }
                    else
                    {
                        throw new Exception("Kon het nieuwe ID niet ophalen.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding facility: {ex.Message}");
                }
            }
        }
        public List<Facility> GetAllFacilities()
        {
            List<Facility> facilities = new();
            string query = "SELECT * FROM Facilities";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = query;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var facility = new Facility(
                            reader.GetInt32(0),
                            reader.GetString(1)
                        );
                        facilities.Add(facility);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving facilities: {ex.Message}");
                }
            }
            return facilities;
        }
        public void SaveFacilieitenLocaties(List<NetworkPoint> networkPoints)
        {
            string query = @"
            INSERT INTO NetworkPointFacilities (NetworkPointId, FacilityId)
            VALUES (@NetworkPointId, @FacilityId);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var point in networkPoints)
                        {
                            foreach (var facility in point.Facilities)
                            {
                                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@NetworkPointId", point.Id);
                                    cmd.Parameters.AddWithValue("@FacilityId", facility.Id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void SaveNetworkPoints(List<NetworkPoint> points)
        {
            string query = @"INSERT INTO NetworkPoints (X, Y) OUTPUT INSERTED.Id VALUES (@X, @Y);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var point in points)
                        {
                            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@X", point.X);
                                cmd.Parameters.AddWithValue("@Y", point.Y);

                                point.Id = (int)cmd.ExecuteScalar();
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public NetworkPoint GetNetworkPointById(int id)
        {
            string query = "SELECT * FROM NetworkPoints WHERE Id = @Id";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.CommandText = query;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new NetworkPoint(reader.GetInt32(0), reader.GetDouble(1), reader.GetDouble(2));
                }
            }
            return null;
        }
        public void SaveSegments(List<List<NetworkPoint>> stretches)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var stretch in stretches)
                {
                    for (int i = 0; i < stretch.Count - 1; i++)
                    {
                        var startPoint = stretch[i];
                        var endPoint = stretch[i + 1];

                        if (startPoint != null && endPoint != null)
                        {
                            using (var cmd = new SqlCommand(@"
                        INSERT INTO Segments (StartPointId, EndPointId)
                        VALUES (@StartPointId, @EndPointId);", connection))
                            {
                                cmd.Parameters.AddWithValue("@StartPointId", startPoint.Id);
                                cmd.Parameters.AddWithValue("@EndPointId", endPoint.Id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            throw new Exception("Network points not found for the provided objects.");
                        }
                    }
                }
            }
        }
        public List<Segment> GetAllSegments()
        {
            List<Segment> segments = new();
            string query = "SELECT * FROM Segments";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = query;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var segment = new Segment(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2)
                        );
                        segments.Add(segment);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving segments: {ex.Message}");
                }
            }
            return segments;
        }
        public void UpdateFacility(Facility facility)
        {
            string query = "UPDATE Facilities SET Naam = @Name WHERE Id = @Id";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.Parameters.AddWithValue("@Id", facility.Id);
                    cmd.Parameters.AddWithValue("@Name", facility.Naam);
                    conn.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating facility: {ex.Message}");
                }
            }
        }
        public void VerwijderFaciliteit(int id)
        {
            string query = "DELETE FROM Facilities WHERE Id = @Id";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error deleting facility: {ex.Message}");
                }
            }
        }
        public List<Facility> GeefFaciliteitenVoorPoint(NetworkPoint point)
        {
            List<Facility> facilities = new();
            string query = @"SELECT n.NetworkPointId, f.Id, f.Naam 
                     FROM Facilities f 
                     JOIN NetworkPointFacilities n ON f.Id = n.FacilityId 
                     WHERE n.NetworkPointId = @id;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", point.Id);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int facilityId = reader.GetInt32(1);
                        string naam = reader.GetString(2);
                        facilities.Add(new Facility(facilityId, naam));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving facilities: {ex.Message}");
                }
            }

            return facilities;
        }
        public void VerwijderPunt(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM NetworkPointFacilities WHERE NetworkPointId = @Id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand("DELETE FROM NetworkPoints WHERE Id = @Id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception($"Error deleting network point: {ex.Message}");
                    }
                }
            }
        }
        public void StelFaciliteitenInVoorPoint(NetworkPoint point, List<Facility> faciliteiten)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var deleteCmd = new SqlCommand(
                            "DELETE FROM NetworkPointFacilities WHERE NetworkPointId = @PointId", conn, tran);
                        deleteCmd.Parameters.AddWithValue("@PointId", point.Id);
                        deleteCmd.ExecuteNonQuery();

                        foreach (var f in faciliteiten)
                        {
                            var insertCmd = new SqlCommand(
                                "INSERT INTO NetworkPointFacilities (NetworkPointId, FacilityId) VALUES (@PointId, @FacilityId)",
                                conn, tran);
                            insertCmd.Parameters.AddWithValue("@PointId", point.Id);
                            insertCmd.Parameters.AddWithValue("@FacilityId", f.Id);
                            insertCmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        public void SaveNetworkPoint(NetworkPoint nieuwPunt)
        {
            string query = @"INSERT INTO NetworkPoints (X, Y) OUTPUT INSERTED.Id VALUES (@X, @Y);";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@X", nieuwPunt.X);
                            cmd.Parameters.AddWithValue("@Y", nieuwPunt.Y);
                            nieuwPunt.Id = (int)cmd.ExecuteScalar();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }
        public void UpdateNetworkPoint(NetworkPoint networkPoint)
        {
            string query = "UPDATE NetworkPoints SET X = @X, Y = @Y WHERE Id = @Id";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.Parameters.AddWithValue("@Id", networkPoint.Id);
                    cmd.Parameters.AddWithValue("@X", networkPoint.X);
                    cmd.Parameters.AddWithValue("@Y", networkPoint.Y);
                    conn.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating network point: {ex.Message}");
                }
            }

        }
        public void SaveSegment(Segment segment)
        {
            string query = "INSERT INTO Segments (StartPointId, EndPointId) OUTPUT INSERTED.Id VALUES (@StartPointId, @EndPointId);";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.Parameters.AddWithValue("@StartPointId", segment.StartPointId);
                    cmd.Parameters.AddWithValue("@EndPointId", segment.EndPointId);
                    conn.Open();
                    cmd.CommandText = query;
                    segment.Id = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error saving segment: {ex.Message}");
                }
            }

        }
        public void VerwijderSegment(int startPointId, int endPointId)
        {
            string query = "DELETE FROM Segments WHERE StartPointId = @StartPointId AND EndPointId = @EndPointId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.Parameters.AddWithValue("@StartPointId", startPointId);
                    cmd.Parameters.AddWithValue("@EndPointId", endPointId);
                    conn.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error deleting segment: {ex.Message}");
                }
            }
        }

    }
}
