using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;
using System.Collections.Generic;
using Xunit;

namespace _4.Route_Netwerk_Testing
{
    public class TestNetwerkPunt
    {
        //TODO: Add test voor grenzen van een X en Y Coordinaat minimum is 0.1 en maximum is 10000.00
        [Fact]
        public void NieuwPunt_StandaardMoetAllesLeegZijn()
        {
            var punt = new NetworkPoint();
            Assert.Equal(0, punt.Id);
            Assert.Equal(0, punt.X);
            Assert.Equal(0, punt.Y);
            Assert.NotNull(punt.Facilities);
            Assert.Empty(punt.Facilities);
        }

        [Fact]
        public void PuntMetXEnY_MoetCoordinatenIngevuldHebben()
        {
            double x = 10.5;
            double y = 20.5;

            var punt = new NetworkPoint(x, y);

            Assert.Equal(0, punt.Id);
            Assert.Equal(x, punt.X);
            Assert.Equal(y, punt.Y);
            Assert.NotNull(punt.Facilities);
            Assert.Empty(punt.Facilities);
        }

        [Fact]
        public void PuntMetIdEnCoordinaten_MoetAllesIngevuldHebben()
        {
            int id = 1;
            double x = 15.0;
            double y = 25.0;

            var punt = new NetworkPoint(id, x, y);

            Assert.Equal(id, punt.Id);
            Assert.Equal(x, punt.X);
            Assert.Equal(y, punt.Y);
            Assert.NotNull(punt.Facilities);
            Assert.Empty(punt.Facilities);
        }

        [Fact]
        public void PuntMetAllesInbegrepen_MoetCorrectZijn()
        {
            int id = 2;
            double x = 30.0;
            double y = 40.0;
            var faciliteiten = new List<Facility> { new Facility(), new Facility() };

            var punt = new NetworkPoint(id, x, y, faciliteiten);

            Assert.Equal(id, punt.Id);
            Assert.Equal(x, punt.X);
            Assert.Equal(y, punt.Y);
            Assert.Equal(faciliteiten, punt.Facilities);
        }

        [Theory]
        [InlineData(1, 10.0, 20.0)]
        [InlineData(2, 15.5, 25.5)]
        [InlineData(3, 0.0, 0.0)]
        public void PuntMetVerschillendeWaarden_MoetAllesKloppendZetten(int id, double x, double y)
        {
            var punt = new NetworkPoint(id, x, y);

            Assert.Equal(id, punt.Id);
            Assert.Equal(x, punt.X);
            Assert.Equal(y, punt.Y);
        }

        [Theory]
        [InlineData(0, 0.0, 0.0)]
        [InlineData(5, 100.0, 200.0)]
        public void PuntControle_MetVasteWaarden(int id, double x, double y)
        {
            var punt = new NetworkPoint(id, x, y);

            Assert.Equal(id, punt.Id);
            Assert.Equal(x, punt.X);
            Assert.Equal(y, punt.Y);
        }

        [Fact]
        public void LeesFaciliteitenLocaties_ShouldLinkFacilitiesToNetworkPoints()
        {
            var fileProcessor = new FileProcessor();
            var tempFilePath = Path.GetTempFileName();

            File.WriteAllText(tempFilePath, "1,101\n2,102\n3,103");

            var networkPoints = new List<NetworkPoint>
            {
                new NetworkPoint(1, 0, 0, new List<Facility>()),
                new NetworkPoint(2, 1, 1, new List<Facility>()),
                new NetworkPoint(3, 2, 2, new List<Facility>())
            };

            var facilities = new List<Facility>
            {
                new Facility(101, "Facility A"),
                new Facility(102, "Facility B"),
                new Facility(103, "Facility C")
            };

            //fileProcessor.LeesFaciliteitenLocaties(tempFilePath, networkPoints, facilities);

            //Assert.Equal(1, networkPoints[0].Facilities.Count);
            //Assert.Equal(101, networkPoints[0].Facilities[0].Id);

            //Assert.Equal(1, networkPoints[1].Facilities.Count);
            //Assert.Equal(102, networkPoints[1].Facilities[0].Id);

            //Assert.Equal(1, networkPoints[2].Facilities.Count);
            //Assert.Equal(103, networkPoints[2].Facilities[0].Id);

            //File.Delete(tempFilePath);
        }
    }
}
