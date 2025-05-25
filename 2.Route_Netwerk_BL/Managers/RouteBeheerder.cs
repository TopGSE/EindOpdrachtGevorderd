using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2.Route_Netwerk_BL.Managers
{
    public class RouteBeheerder
    {
        private IRouteRepository repo;

        public RouteBeheerder(IRouteRepository repository)
        {
            repo = repository;
        }

        public List<Route> GetAllRoutes()
        {
            return repo.GetAllRoutes();
        }

        public void MaakNieuweRoute(string naam, List<NetworkPoint> punten)
        {
            if (string.IsNullOrWhiteSpace(naam) || naam.Length < 3)
                throw new ArgumentException("Route naam moet minstens 3 karakters bevatten.");
            if (punten == null || punten.Count < 5)
                throw new ArgumentException("Je moet minstens 5 punten selecteren.");

            punten[0].IsStopPlaats = true;
            punten[^1].IsStopPlaats = true;

            repo.VoegRouteToe(naam, punten);

        }

        public Route GetRouteById(int id)
        {
           return repo.GetRouteById(id);
        }

        public void UpdateRoute(Route loadedRoute)
        {
            repo.UpdateRoute(loadedRoute);
        }

        public void DeleteRoute(int id)
        {
            repo.DeleteRoute(id);
        }
        //TODO: Fix the query in the repository and implement the methods in de wpf voor route te updaten
        //TODO: Gebruik mappers in de routewpf zodat de veranderingen zichtbaar worden in de UI

    }
}
