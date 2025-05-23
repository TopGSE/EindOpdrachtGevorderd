using _2.Route_Netwerk_BL.Exceptions;
using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _3.Route_Netwerk_DL
{
    public class FileProcessor
    {
        
        private NetwerkBeheerder beheerder;
        private Dictionary<int, NetworkPoint> _location = new Dictionary<int, NetworkPoint>();
        private Dictionary<int, Facility> _facility = new Dictionary<int, Facility>();

        public List<NetworkPoint> LeesNetworkPoints(string pad)
        {
            List<NetworkPoint> points = new List<NetworkPoint>();
            using (StreamReader reader = new StreamReader(pad))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        string[] parts = line.Split('|');
                        int id = int.Parse(parts[0]);
                        double x = double.Parse(parts[1]);
                        double y = double.Parse(parts[2]);

                        NetworkPoint point = new NetworkPoint(id, x, y);
                        points.Add(point);

                    }
                    catch (Exception ex)
                    {
                        throw new NetwerkException($"Error reading network point from file: {ex.Message}");
                    }
                }
            }
            return points;
        }



        public List<Facility> LeesFacilities(string pad)
        {
            List<Facility> facilities = new();
            using (StreamReader reader = new StreamReader(pad))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        string[] parts = line.Split(',');
                        string facilityName = parts[1];  

                        Facility facility = new Facility(facilityName);

                        if (!facilities.Any(f => f.Naam == facility.Naam))
                        {
                            facilities.Add(facility);
                        }
                    }
                    catch (Exception ex) 
                    {
                        throw new NetwerkException($"Error reading facility from file: {ex.Message}");
                    }
                }
            }

            return facilities;
        }

        public void LeesFaciliteitenLocaties(string pad)
        {
            try
            {
                using (StreamReader reader = new StreamReader(pad))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        int locationId = int.Parse(parts[0]);
                        int facilityId = int.Parse(parts[1]);
                        _location[locationId].Facilities.Add(_facility[facilityId]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ReadFacilityLocation Mislukt! ", ex);
            }
        }


        public List<List<int>> LeesStretchDataIn(string pad)
        {
            var stretches = new List<List<int>>();
            var lines = File.ReadAllLines(pad);

            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (lines[i].StartsWith("stretch"))
                {
                    string puntenRegel = lines[i + 1];
                    var matches = Regex.Matches(puntenRegel, @"\((\d+)\)");
                    var pointIds = matches.Select(m => int.Parse(m.Groups[1].Value)).ToList();
                    stretches.Add(pointIds);
                }
            }
            return stretches;
        }

        public void SetData(List<NetworkPoint> locaties, List<Facility> faciliteiten)
        {
            _location = locaties.ToDictionary(l => l.Id);
            _facility = faciliteiten.ToDictionary(f => f.Id);
        }

    }
}
