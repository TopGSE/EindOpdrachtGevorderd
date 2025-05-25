using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2.Route_Netwerk_BL.Managers
{
    public class NetwerkBeheerder
    {
        private readonly INetwerkRepository repo;

        public NetwerkBeheerder(INetwerkRepository repository)
        {
            repo = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // Facility Methods
        public List<Facility> GetAllFacilities() => repo.GetAllFacilities();

        public void VoegFaciliteitToe(Facility facility)
        {
            if (facility == null) throw new ArgumentNullException(nameof(facility));

            var existingFacilities = repo.GetAllFacilities();
            if (existingFacilities.Any(f =>
                string.Equals(f.Naam, facility.Naam, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A facility with the same name already exists.");
            }

            repo.VoegFaciliteitToe(facility);
        }

        public void UpdateFacility(Facility facility)
        {
            if (facility == null) throw new ArgumentNullException(nameof(facility));

            var existingFacilities = repo.GetAllFacilities();
            if (existingFacilities.Any(f =>
                string.Equals(f.Naam, facility.Naam, StringComparison.OrdinalIgnoreCase) && f.Id != facility.Id))
            {
                throw new InvalidOperationException("A facility with the same name already exists.");
            }

            var networkPoints = repo.GetNetworkPoints();
            bool isLinked = networkPoints.Any(np => np.Facilities?.Any(f => f.Id == facility.Id) == true);
            if (isLinked)
            {
                throw new InvalidOperationException("Cannot update facility that is linked to a network point.");
            }

            repo.UpdateFacility(facility);
        }

        public void VerwijderFaciliteit(int id)
        {
            repo.VerwijderFaciliteit(id);
        }

        public List<Facility> GeefFaciliteitenVoorPoint(NetworkPoint point)
        {
            return repo.GeefFaciliteitenVoorPoint(point);
        }

        public void StelFaciliteitenInVoorPoint(NetworkPoint domeinPunt, List<Facility> facilities)
        {
            repo.StelFaciliteitenInVoorPoint(domeinPunt, facilities);
        }

        // Network Point Methods
        public List<NetworkPoint> GetNetworkPoints() => repo.GetNetworkPoints();

        public NetworkPoint GetNetworkPointById(int id) => repo.GetNetworkPointById(id);

        public void SaveNetworkPoint(NetworkPoint nieuwPunt)
        {
            repo.SaveNetworkPoint(nieuwPunt);
        }

        public void UpdateNetworkPoint(NetworkPoint networkPoint)
        {
            if (networkPoint == null) throw new ArgumentNullException(nameof(networkPoint));
            if (networkPoint.X == networkPoint.Y)
                throw new InvalidOperationException("X and Y coordinates cannot be the same.");
            repo.UpdateNetworkPoint(networkPoint);
        }

        public void DeletePoint(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID", nameof(id));

            var segments = repo.GetAllSegments();
            if (segments.Any(s => s.StartPointId == id || s.EndPointId == id))
            {
                throw new InvalidOperationException("Je kan geen punt verwijderen die aan een segment verbonden is!");
            }

            repo.VerwijderPunt(id);
        }

        public void SaveFacilieitenLocaties(List<NetworkPoint> networkPoints)
        {
            repo.SaveNetworkPoints(networkPoints);
        }

        // Segment Methods
        public List<Segment> GetAllSegments() => repo.GetAllSegments();

        public void SaveSegment(Segment segment)
        {
            repo.SaveSegment(segment);
        }

        public void SaveSegments(List<List<NetworkPoint>> stretches)
        {
            repo.SaveSegments(stretches);
        }

        public void AddSegment(int startPointId, int endPointId)
        {
            if (startPointId == endPointId)
                throw new InvalidOperationException("Je kunt geen verbinding maken met hetzelfde punt.");

            var segments = repo.GetAllSegments();
            bool bestaatAl = segments.Any(s =>
                (s.StartPointId == startPointId && s.EndPointId == endPointId) ||
                (s.StartPointId == endPointId && s.EndPointId == startPointId));

            if (bestaatAl)
                throw new InvalidOperationException("Er bestaat al een verbinding tussen deze twee punten.");

            repo.SaveSegment(new Segment { StartPointId = startPointId, EndPointId = endPointId });
        }

        public void RemoveSegment(int startPointId, int endPointId)
        {
            var segments = repo.GetAllSegments();
            bool bestaat = segments.Any(s =>
                (s.StartPointId == startPointId && s.EndPointId == endPointId) ||
                (s.StartPointId == endPointId && s.EndPointId == startPointId));

            if (!bestaat)
                throw new InvalidOperationException("Er bestaat geen verbinding tussen deze twee punten.");

            repo.VerwijderSegment(startPointId, endPointId);
        }

        public void VerwijderSegment(int startPointId, int endPointId)
        {
            repo.VerwijderSegment(startPointId, endPointId);
        }
    }
}
