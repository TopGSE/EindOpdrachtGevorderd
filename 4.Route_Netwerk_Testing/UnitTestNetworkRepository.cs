using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4.Route_Netwerk_Testing
{
    public class UnitTestNetworkRepository
    {
        public class NetwerkRepositoryTests
        {
            private readonly NetwerkRepository _repo;

            public NetwerkRepositoryTests()
            {
                _repo = new NetwerkRepository();
            }

            [Fact]
            public void GetNetworkPoints_ReturnsList()
            {
                var result = _repo.GetNetworkPoints();
                Assert.NotNull(result);
            }

            [Theory]
            [InlineData("Park")]
            [InlineData("Toilet")]
            public void VoegFaciliteitToe_AddsFacility(string name)
            {
                var facility = new Facility(0, name);
                _repo.VoegFaciliteitToe(facility);
                Assert.True(facility.Id > 0);
            }

            [Fact]
            public void GetAllFacilities_ReturnsFacilities()
            {
                var result = _repo.GetAllFacilities();
                Assert.NotNull(result);
            }

            [Fact]
            public void SaveFacilieitenLocaties_SavesWithoutError()
            {
                var point = new NetworkPoint(0, 10.0, 20.0);
                _repo.SaveNetworkPoint(point);
                var facility = new Facility(0, "TestFacility");
                _repo.VoegFaciliteitToe(facility);
                point.Facilities = new List<Facility> { facility };

                _repo.SaveFacilieitenLocaties(new List<NetworkPoint> { point });
            }

            [Fact]
            public void SaveNetworkPoints_SavesMultiple()
            {
                var list = new List<NetworkPoint>
        {
            new NetworkPoint(0, 1.1, 1.2),
            new NetworkPoint(0, 2.1, 2.2)
        };
                _repo.SaveNetworkPoints(list);

                Assert.All(list, p => Assert.True(p.Id > 0));
            }

            [Fact]
            public void GetNetworkPointById_ReturnsCorrect()
            {
                var point = new NetworkPoint(0, 100, 200);
                _repo.SaveNetworkPoint(point);

                var result = _repo.GetNetworkPointById(point.Id);
                Assert.NotNull(result);
                Assert.Equal(point.Id, result.Id);
            }

            [Fact]
            public void SaveSegments_SavesCorrectly()
            {
                var a = new NetworkPoint(0, 0, 0);
                var b = new NetworkPoint(0, 1, 1);
                _repo.SaveNetworkPoints(new List<NetworkPoint> { a, b });

                _repo.SaveSegments(new List<List<NetworkPoint>> { new List<NetworkPoint> { a, b } });
            }

            [Fact]
            public void GetAllSegments_ReturnsList()
            {
                var result = _repo.GetAllSegments();
                Assert.NotNull(result);
            }

            [Fact]
            public void UpdateFacility_UpdatesSuccessfully()
            {
                var facility = new Facility(0, "OldName");
                _repo.VoegFaciliteitToe(facility);

                facility.Naam = "NewName";
                _repo.UpdateFacility(facility);

                var result = _repo.GetAllFacilities().Find(f => f.Id == facility.Id);
                Assert.Equal("NewName", result.Naam);
            }

            [Fact]
            public void VerwijderFaciliteit_DeletesSuccessfully()
            {
                var facility = new Facility(0, "ToDelete");
                _repo.VoegFaciliteitToe(facility);

                _repo.VerwijderFaciliteit(facility.Id);
                var facilities = _repo.GetAllFacilities();

                Assert.DoesNotContain(facilities, f => f.Id == facility.Id);
            }

            [Fact]
            public void GeefFaciliteitenVoorPoint_ReturnsCorrect()
            {
                var point = new NetworkPoint(0, 0, 0);
                var facility = new Facility(0, "F1");
                _repo.SaveNetworkPoint(point);
                _repo.VoegFaciliteitToe(facility);
                _repo.StelFaciliteitenInVoorPoint(point, new List<Facility> { facility });

                var facilities = _repo.GeefFaciliteitenVoorPoint(point);
                Assert.Contains(facilities, f => f.Id == facility.Id);
            }

            [Fact]
            public void VerwijderPunt_RemovesCorrectly()
            {
                var point = new NetworkPoint(0, 1, 1);
                _repo.SaveNetworkPoint(point);

                _repo.VerwijderPunt(point.Id);
                var result = _repo.GetNetworkPointById(point.Id);
                Assert.Null(result);
            }

            [Fact]
            public void StelFaciliteitenInVoorPoint_SetsFacilities()
            {
                var point = new NetworkPoint(0, 5, 5);
                var facility = new Facility(0, "Assigned");
                _repo.SaveNetworkPoint(point);
                _repo.VoegFaciliteitToe(facility);

                _repo.StelFaciliteitenInVoorPoint(point, new List<Facility> { facility });
                var facilities = _repo.GeefFaciliteitenVoorPoint(point);
                Assert.Contains(facilities, f => f.Id == facility.Id);
            }

            [Fact]
            public void SaveNetworkPoint_SavesOnePoint()
            {
                var point = new NetworkPoint(0, 3.3, 4.4);
                _repo.SaveNetworkPoint(point);
                Assert.True(point.Id > 0);
            }

            [Fact]
            public void UpdateNetworkPoint_UpdatesSuccessfully()
            {
                var point = new NetworkPoint(0, 1.0, 1.0);
                _repo.SaveNetworkPoint(point);

                point.X = 9.9;
                point.Y = 8.8;
                _repo.UpdateNetworkPoint(point);

                var updated = _repo.GetNetworkPointById(point.Id);
                Assert.Equal(9.9, updated.X);
                Assert.Equal(8.8, updated.Y);
            }

            [Fact]
            public void SaveSegment_SavesSegment()
            {
                var a = new NetworkPoint(0, 0, 0);
                var b = new NetworkPoint(0, 1, 1);
                _repo.SaveNetworkPoints(new List<NetworkPoint> { a, b });

                var segment = new Segment(0, a.Id, b.Id);
                _repo.SaveSegment(segment);
                Assert.True(segment.Id > 0);
            }

            [Fact]
            public void VerwijderSegment_DeletesCorrectly()
            {
                var a = new NetworkPoint(0, 0, 0);
                var b = new NetworkPoint(0, 1, 1);
                _repo.SaveNetworkPoints(new List<NetworkPoint> { a, b });
                var segment = new Segment(0, a.Id, b.Id);
                _repo.SaveSegment(segment);

                _repo.VerwijderSegment(a.Id, b.Id);
                var segments = _repo.GetAllSegments();
                Assert.DoesNotContain(segments, s => s.StartPointId == a.Id && s.EndPointId == b.Id);
            }
        }
    }
}
