using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Managers
{
    public class NetwerkBeheerder
    {
        private INetwerkRepository repo;

        public NetwerkBeheerder(INetwerkRepository repository)
        {
            repo = repository;
        }

        public void SaveFacilieitenLocaties(List<NetworkPoint> networkPoints)
        {
            repo.SaveNetworkPoints(networkPoints);
        }
        public void VoegFaciliteitToe(Facility facility)
        {
            var existingFacilities = repo.GetAllFacilities();
            if (existingFacilities.Any(f => f.Naam.ToLower().Equals(facility.Naam.ToLower(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A facility with the same name already exists.");
            }
            repo.VoegFaciliteitToe(facility);
        }
        public List<Facility> GetAllFacilities()
        {
            return repo.GetAllFacilities();
        }
        public void SaveNetworkPoints(List<NetworkPoint> points)
        {
            repo.SaveNetworkPoints(points);
        }
        public void SaveSegments(List<List<NetworkPoint>> stretches)
        {
            repo.SaveSegments(stretches);
        }
        public List<NetworkPoint> GetNetworkPoints()
        {
            return repo.GetNetworkPoints();
        }
        public List<Segment> GetAllSegments()
        {
            return repo.GetAllSegments();
        }
        public void UpdateFacility(Facility facility)
        {
            var existingFacilities = repo.GetAllFacilities();
            if (existingFacilities.Any(f =>
                string.Equals(f.Naam, facility.Naam, StringComparison.OrdinalIgnoreCase) && f.Id != facility.Id))
            {
                throw new InvalidOperationException("A facility with the same name already exists.");
            }

            var networkPoints = repo.GetNetworkPoints();
            bool isLinked = networkPoints.Any(np => np.Facilities != null && np.Facilities.Any(f => f.Id == facility.Id));
            if (isLinked)
            {
                throw new InvalidOperationException("Cannot update facility that is linked to a network point.");
            }

            repo.UpdateFacility(facility);
        }
        public void VerwijderFaciliteit(int id)
        {
            //var networkPoints = repo.GetNetworkPoints();
            //bool isLinked = networkPoints.Any(np => np.Facilities != null && np.Facilities.Any(f => f.Id == id));
            //if (isLinked)
            //{
            //    throw new InvalidOperationException("Cannot delete facility that is linked to a network point.");
            //}
            repo.VerwijderFaciliteit(id);
        }
        public NetworkPoint GetNetworkPointById(int id)
        {
            return repo.GetNetworkPointById(id);
        }
        public List<Facility> GeefFaciliteitenVoorPoint(NetworkPoint point)
        {
            return repo.GeefFaciliteitenVoorPoint(point);
        }
        public void VerwijderPunt(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID", nameof(id));
            var segments = repo.GetAllSegments();
            if (segments.Any(s => s.StartPointId == id || s.EndPointId == id))
            {
                throw new InvalidOperationException("Cannot delete point that is part of a segment.");
            }
            repo.VerwijderPunt(id);
        }
        public void StelFaciliteitenInVoorPoint(NetworkPoint domeinPunt, List<Facility> facilities)
        {
            repo.StelFaciliteitenInVoorPoint(domeinPunt, facilities);
        }
        public void SaveNetworkPoint(NetworkPoint nieuwPunt)
        {
            repo.SaveNetworkPoint(nieuwPunt);
        }
        public void UpdateNetworkPoint(NetworkPoint networkPoint)
        {
            repo.UpdateNetworkPoint(networkPoint);
        }
        public void SaveSegment(Segment segment)
        {
            repo.SaveSegment(segment);
        }
        public void VerwijderSegment(int startPointId, int endPointId)
        {
            repo.VerwijderSegment(startPointId, endPointId);
        }
    }
}
