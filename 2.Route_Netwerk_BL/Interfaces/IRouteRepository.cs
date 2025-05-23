using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Interfaces
{
    public interface IRouteRepository
    {
        List<Route> GetAllRoutes();
        void VoegRouteToe(string naam, List<NetworkPoint> punten);
        List<NetworkPoint> HaalPuntenOp(int id);
        Route GetRouteById(int id);
        void UpdateRoute(Route loadedRoute);
    }
}
