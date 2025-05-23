using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Models
{
    public class Route
    {
        public Route(string naam)
        {
            Naam = naam;
        }

        public Route(int id, string naam)
        {
            Id = id;
            Naam = naam;
        }

        public Route(string naam, List<NetworkPoint> punten) : this(naam)
        {
            Punten = punten;
        }

        public Route(int id, string naam, List<NetworkPoint> punten) : this(id, naam)
        {
            Punten = punten;
        }

        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public List<NetworkPoint> Punten { get; set; } = new();
    }
}
