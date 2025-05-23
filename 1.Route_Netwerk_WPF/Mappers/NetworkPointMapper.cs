using _1.Route_Netwerk_WPF.Models;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Route_Netwerk_WPF.Dto
{
    public class NetworkPointMapper
    {
        public static NetworkPointUI MapToUI(NetworkPoint networkPoint)
        {
           return new NetworkPointUI(networkPoint.Id, networkPoint.X, networkPoint.Y);
        }
        public static NetworkPoint MapToDomain(NetworkPointUI networkPointUI)
        {
            return new NetworkPoint(networkPointUI.Id, networkPointUI.X, networkPointUI.Y);
        }
    }
}
