using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Models
{
    public class Facility
    {
        public Facility()
        {
        }

        public Facility(string naam)
        {
            Naam = naam;
        }

        public Facility(int id, string name)
        {
            Id = id;
            Naam = name;
        }

        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;

        public override string ToString()
        {
            return $" {Id} - {Naam}";
        }
    }
}
