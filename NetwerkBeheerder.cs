using System;
using System.Collections.Generic;
using System.Linq;

namespace _2.Route_Netwerk_BL.Managers
{
    public class NetwerkBeheerder
    {
        private List<NetworkPoint> _networkPoints = new();
        private List<(int Point1Id, int Point2Id)> _connections = new();
        private List<Facility> _facilities = new();

        public void AddNetworkPoint(double x, double y, List<Facility> facilities)
        {
            int newId = _networkPoints.Any() ? _networkPoints.Max(p => p.Id) + 1 : 1;
            var newPoint = new NetworkPoint(newId, x, y, facilities);
            _networkPoints.Add(newPoint);
        }

        public void RemoveNetworkPoint(int pointId)
        {
            _networkPoints.RemoveAll(p => p.Id == pointId);
            _connections.RemoveAll(c => c.Point1Id == pointId || c.Point2Id == pointId);
        }

        public void UpdateNetworkPoint(int pointId, double newX, double newY, List<Facility> newFacilities)
        {
            var point = _networkPoints.FirstOrDefault(p => p.Id == pointId);
            if (point != null)
            {
                point.X = newX;
                point.Y = newY;
                point.Facilities = newFacilities;
            }
        }

        public void AddConnection(int point1Id, int point2Id)
        {
            if (_networkPoints.Any(p => p.Id == point1Id) && _networkPoints.Any(p => p.Id == point2Id))
            {
                _connections.Add((point1Id, point2Id));
            }
        }

        public void RemoveConnection(int point1Id, int point2Id)
        {
            _connections.RemoveAll(c => (c.Point1Id == point1Id && c.Point2Id == point2Id) ||
                                        (c.Point1Id == point2Id && c.Point2Id == point1Id));
        }
    }
}
