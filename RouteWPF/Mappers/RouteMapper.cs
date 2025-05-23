using _2.Route_Netwerk_BL.Models;
using RouteWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteWPF.Mappers
{
    public class RouteMapper
    {
        public static RouteUI MapToUI(Route route)
        {
            return new RouteUI(route.Id, route.Naam);
        }

        public static Route MapToDomain(RouteUI routeUI)
        {
            return new Route(routeUI.Id, routeUI.Naam);
        }
    }
}
