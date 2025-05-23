using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Models
{
    public class NetworkPoint
    {
        public NetworkPoint()
        {
        }

        public NetworkPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public NetworkPoint(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public NetworkPoint(double x, double y, List<Facility> facilities) : this(x, y)
        {
            Facilities = facilities;
        }

        public NetworkPoint(int id, double x, double y, List<Facility> facilities, bool isStopPlaats)
        {
            Id = id;
            X = x;
            Y = y;
            Facilities = facilities;
            IsStopPlaats = isStopPlaats;
        }

        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsStopPlaats { get; set; }
        public List<Facility> Facilities { get; set; } = new();

        public override string ToString()
        {
            return $"({X}, {Y}) Stop: {IsStopPlaats}";
        }
    }
}
