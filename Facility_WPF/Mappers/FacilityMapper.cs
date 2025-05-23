using _1.Route_Netwerk_WPF.Models;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Route_Netwerk_WPF.Mappers
{
    public class FacilityMapper
    {
        public static Facility MapToDomain(FacilityUI fac)
        {
            return new Facility(fac.Id, fac.Name);
        }

        public static FacilityUI MapToUI(Facility fac)
        {
            return new FacilityUI(fac.Id, fac.Naam);
        }

    }
}
