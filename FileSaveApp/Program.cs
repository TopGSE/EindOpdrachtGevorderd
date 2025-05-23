// See https://aka.ms/new-console-template for more information
using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;

string faciliteitenLocatiesPad = @"C:\Users\Ekovs\Downloads\faciliteiten_locaties.txt";
string networkPointsPad = @"C:\Users\Ekovs\Downloads\network_points.txt";
string segmentsPad = @"C:\Users\Ekovs\Downloads\network_stretches.txt";

void Placeholder()
{
    //NetwerkRepository repo = new NetwerkRepository();
    //FileProcessor fileProcessor = new FileProcessor();

    //List<Facility> faciliteiten = repo.GetAllFacilities();
    //List<NetworkPoint> locaties = repo.GetNetworkPoints();

    //fileProcessor.SetData(locaties, faciliteiten);

    //fileProcessor.LeesFaciliteitenLocaties(faciliteitenLocatiesPad);

    //repo.SaveFacilieitenLocaties(locaties);

    //Console.WriteLine("Klaar!");

    //FileProcessor fileProcessor = new FileProcessor();
    //NetwerkRepository repo = new NetwerkRepository();

    //List<NetworkPoint> allePoints = repo.GetNetworkPoints();

    //List<List<int>> stretchIds = fileProcessor.LeesStretchDataIn(segmentsPad);

    //List<List<NetworkPoint>> stretches = new List<List<NetworkPoint>>();

    //foreach (var stretch in stretchIds)
    //{
    //    List<NetworkPoint> connectedPoints = new List<NetworkPoint>();

    //    foreach (int id in stretch)
    //    {
    //        NetworkPoint point = allePoints.FirstOrDefault(p => p.Id == id);
    //        if (point == null)
    //        {
    //            throw new Exception($"NetworkPoint met ID {id} niet gevonden!");
    //        }
    //        connectedPoints.Add(point);
    //    }

    //    stretches.Add(connectedPoints);
    //}

    //repo.SaveSegments(stretches);

    //Console.WriteLine("Klaar!");
}


//List<Facility> faciliteiten = new List<Facility>
//{
//    new Facility(1, "Faciliteit A"),
//    new Facility(2, "Faciliteit B"),
//    new Facility(3, "Faciliteit C")
//};
//NetworkPoint n = new NetworkPoint(3, 276.8999, 185.999, faciliteiten);

//NetwerkRepository repo = new NetwerkRepository();
//foreach(var faciliteit in repo.GeefFaciliteitenVoorPoint(n))
//{
//    Console.WriteLine(faciliteit);
//}
