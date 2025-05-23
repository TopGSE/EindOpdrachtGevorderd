using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Interfaces
{
    public interface INetwerkRepository
    {
        List<Facility> GetAllFacilities();
        void SaveNetworkPoints(List<NetworkPoint> points);
        void VoegFaciliteitToe(Facility facility);
        public void SaveSegments(List<List<NetworkPoint>> stretches);
        public List<NetworkPoint> GetNetworkPoints();
        List<Segment> GetAllSegments();
        void UpdateFacility(Facility facility);
        void VerwijderFaciliteit(int id);
        NetworkPoint GetNetworkPointById(int id);
        List<Facility> GeefFaciliteitenVoorPoint(NetworkPoint point);
        void VerwijderPunt(int id);
        void StelFaciliteitenInVoorPoint(NetworkPoint domeinPunt, List<Facility> facilities);
        void SaveNetworkPoint(NetworkPoint nieuwPunt);
        void UpdateNetworkPoint(NetworkPoint networkPoint);
        void SaveSegment(Segment segment);
        void VerwijderSegment(int startPointId, int endPointId);
    }
}
