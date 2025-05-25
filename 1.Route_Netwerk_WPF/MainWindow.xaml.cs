using _1.Route_Netwerk_WPF.Dto;
using _1.Route_Netwerk_WPF.Models;
using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _1.Route_Netwerk_WPF
{
    public partial class MainWindow : Window
    {
        private NetwerkBeheerder netwerkBeheerder;
        private double minX, maxX, minY, maxY, scale;
        private Dictionary<int, Ellipse> pointEllipses = new();
        private ObservableCollection<Ellipse> selectedEllipses = new();
        private ObservableCollection<SegmentUI> segmentData = new();
        private ObservableCollection<NetworkPointUI> points = new();
        private List<Ellipse> routeEllipses = new();
        private bool isRouteShown = false;
        private List<Line> segmentLines = new();
        private int? actiefPuntId = null;
        private bool isAddPointMode = false;

        public MainWindow()
        {
            InitializeComponent();
            netwerkBeheerder = new NetwerkBeheerder(new NetwerkRepository());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNetwork();
        }

        private void LoadNetwork()
        {
            try
            {
                points = new ObservableCollection<NetworkPointUI>(
                    netwerkBeheerder.GetNetworkPoints().Select(NetworkPointMapper.MapToUI));

                segmentData = new ObservableCollection<SegmentUI>(
                    netwerkBeheerder.GetAllSegments().Select(SegmentMapper.MapToUI));

                if (!points.Any()) return;

                minX = points.Min(p => p.X);
                maxX = points.Max(p => p.X);
                minY = points.Min(p => p.Y);
                maxY = points.Max(p => p.Y);

                double canvasWidth = NetworkCanvas.ActualWidth;
                double canvasHeight = NetworkCanvas.ActualHeight;

                double scaleX = canvasWidth / (maxX - minX);
                double scaleY = canvasHeight / (maxY - minY);
                scale = Math.Min(scaleX, scaleY);

                NetworkCanvas.Children.Clear();
                pointEllipses.Clear();
                segmentLines.Clear();

                foreach (var point in points)
                {
                    DrawNetworkPoint(point);
                }

                foreach (var segment in segmentData)
                {
                    DrawSegment(segment, points);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het laden van het netwerk: {ex.Message}");
            }
        }
        private void DrawNetworkPoint(NetworkPointUI point)
        {
            double scaledX = (point.X - minX) * scale;
            double scaledY = (point.Y - minY) * scale;

            var ellipse = new Ellipse
            {
                Width = 5,
                Height = 5,
                Stroke = Brushes.Black,
                Fill = Brushes.Red,
                Tag = point.Id,
                ContextMenu = (ContextMenu)this.Resources["PointContextMenu"]
            };

            ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            ellipse.ContextMenuOpening += (s, e) => actiefPuntId = point.Id;

            Canvas.SetLeft(ellipse, scaledX);
            Canvas.SetTop(ellipse, scaledY);
            NetworkCanvas.Children.Add(ellipse);

            pointEllipses[point.Id] = ellipse;
        }
        private void DrawSegment(SegmentUI segment, IEnumerable<NetworkPointUI> networkPoints)
        {
            var startPoint = networkPoints.FirstOrDefault(p => p.Id == segment.StartPointId);
            var endPoint = networkPoints.FirstOrDefault(p => p.Id == segment.EndPointId);

            if (startPoint != null && endPoint != null)
            {
                double offset = 2.5;

                double startX = (startPoint.X - minX) * scale + offset;
                double startY = (startPoint.Y - minY) * scale + offset;
                double endX = (endPoint.X - minX) * scale + offset;
                double endY = (endPoint.Y - minY) * scale + offset;

                var line = new Line
                {
                    X1 = startX,
                    Y1 = startY,
                    X2 = endX,
                    Y2 = endY,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                NetworkCanvas.Children.Add(line);
                Canvas.SetZIndex(line, -1);
                segmentLines.Add(line);
            }
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse clickedEllipse && clickedEllipse.Tag is int pointId)
            {
                if (selectedEllipses.Contains(clickedEllipse))
                {
                    clickedEllipse.Fill = Brushes.Red;
                    clickedEllipse.Width = 5;
                    clickedEllipse.Height = 5;
                    selectedEllipses.Remove(clickedEllipse);
                }
                else
                {
                    clickedEllipse.Fill = Brushes.Blue;
                    clickedEllipse.Width = 7;
                    clickedEllipse.Height = 7;
                    selectedEllipses.Add(clickedEllipse);
                }

                foreach (var line in segmentLines)
                {
                    line.Stroke = Brushes.Black;
                }

                var selectedIds = selectedEllipses.Select(el => (int)el.Tag).ToHashSet();

                for (int i = 0; i < segmentData.Count; i++)
                {
                    var segment = segmentData[i];

                    if (selectedIds.Contains(segment.StartPointId) && selectedIds.Contains(segment.EndPointId))
                    {
                        segmentLines[i].Stroke = Brushes.Green;
                    }
                }
            }
        }
        private void BewerkPunt_Click(object sender, RoutedEventArgs e)
        {
            if (actiefPuntId is int id)
            {
                var selectedPoint = points.FirstOrDefault(p => p.Id == id);
                if (selectedPoint != null)
                {
                    var editWindow = new NetworkWindow(selectedPoint, netwerkBeheerder);
                    bool? result = editWindow.ShowDialog();

                    if (result == true)
                    {
                        try
                        {
                            netwerkBeheerder.UpdateNetworkPoint(NetworkPointMapper.MapToDomain(selectedPoint));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        if (pointEllipses.TryGetValue(selectedPoint.Id, out var ellipse))
                        {
                            double scaledX = (selectedPoint.X - minX) * scale;
                            double scaledY = (selectedPoint.Y - minY) * scale;
                            Canvas.SetLeft(ellipse, scaledX);
                            Canvas.SetTop(ellipse, scaledY);
                        }
                        UpdateConnectedSegments(selectedPoint.Id);
                        LoadNetwork();
                        //TODO: voeg punt in de lijst terug 
                        //referentie probleem
                        
                    }
                }
            }
        }
        private void VoegVerbindingToe_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEllipses.Count != 2)
            {
                MessageBox.Show("Selecteer precies twee punten om een verbinding te maken.");
                return;
            }

            int startId = (int)selectedEllipses[0].Tag;
            int endId = (int)selectedEllipses[1].Tag;

            try
            {
                netwerkBeheerder.AddSegment(startId, endId);

                var nieuwSegment = new SegmentUI { StartPointId = startId, EndPointId = endId };
                segmentData.Add(nieuwSegment);
                DrawSegment(nieuwSegment, points);

                foreach (var ellipse in selectedEllipses)
                {
                    ellipse.Fill = Brushes.Red;
                    ellipse.Width = 5;
                    ellipse.Height = 5;
                }
                selectedEllipses.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void VerwijderVerbinding_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEllipses.Count != 2)
            {
                MessageBox.Show("Selecteer precies twee punten om een verbinding te verwijderen.");
                return;
            }

            int id1 = (int)selectedEllipses[0].Tag;
            int id2 = (int)selectedEllipses[1].Tag;

            try
            {
                netwerkBeheerder.RemoveSegment(id1, id2);

                int index = segmentData.ToList().FindIndex(s =>
                    (s.StartPointId == id1 && s.EndPointId == id2) ||
                    (s.StartPointId == id2 && s.EndPointId == id1));

                if (index != -1)
                {
                    NetworkCanvas.Children.Remove(segmentLines[index]);
                    segmentLines.RemoveAt(index);
                    segmentData.RemoveAt(index);
                }

                foreach (var ellipse in selectedEllipses)
                {
                    ellipse.Fill = Brushes.Red;
                    ellipse.Width = 5;
                    ellipse.Height = 5;
                }
                selectedEllipses.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UpdateConnectedSegments(int pointId)
        {
            for (int i = 0; i < segmentData.Count; i++)
            {
                var segment = segmentData[i];
                if (segment.StartPointId == pointId || segment.EndPointId == pointId)
                {
                    var startPoint = points.FirstOrDefault(p => p.Id == segment.StartPointId);
                    var endPoint = points.FirstOrDefault(p => p.Id == segment.EndPointId);
                    if (startPoint != null && endPoint != null)
                    {
                        double offset = 2.5;

                        double startX = (startPoint.X - minX) * scale + offset;
                        double startY = (startPoint.Y - minY) * scale + offset;
                        double endX = (endPoint.X - minX) * scale + offset;
                        double endY = (endPoint.Y - minY) * scale + offset;

                        segmentLines[i].X1 = startX;
                        segmentLines[i].Y1 = startY;
                        segmentLines[i].X2 = endX;
                        segmentLines[i].Y2 = endY;
                    }
                }
            }
        }
        private void VerwijderPunt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (actiefPuntId is int id)
                {
                    netwerkBeheerder.DeletePoint(id);

                    if (pointEllipses.TryGetValue(id, out var ellipse))
                    {
                        NetworkCanvas.Children.Remove(ellipse);
                        pointEllipses.Remove(id);
                    }

                    for (int i = segmentData.Count - 1; i >= 0; i--)
                    {
                        var segment = segmentData[i];
                        if (segment.StartPointId == id || segment.EndPointId == id)
                        {
                            NetworkCanvas.Children.Remove(segmentLines[i]);
                            segmentLines.RemoveAt(i);
                            segmentData.RemoveAt(i);
                        }
                    }

                    points.Remove(points.First(p => p.Id == id));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void VoegPuntToe_Click(object sender, RoutedEventArgs e)
        {
            isAddPointMode = true;
            this.Cursor = Cursors.Cross;
            MessageBox.Show("Klik op het canvas om een nieuw punt toe te voegen.");
        }
        private void NetworkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isAddPointMode)
                return;

            if (scale == 0)
            {
                MessageBox.Show("Canvas nog niet klaar. Laad eerst het netwerk.");
                return;
            }

            Point pos = e.GetPosition(NetworkCanvas);
            double x = pos.X / scale + minX;
            double y = pos.Y / scale + minY;

            var nieuwPunt = new NetworkPointUI { X = x, Y = y };

            DrawNetworkPoint(nieuwPunt);

            try
            {
                netwerkBeheerder.SaveNetworkPoint(NetworkPointMapper.MapToDomain(nieuwPunt));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                NetworkCanvas.Children.Remove(NetworkCanvas.Children[^1]);
                isAddPointMode = false;
                this.Cursor = Cursors.Arrow;
                return;
            }

            var window = new NetworkWindow(nieuwPunt, netwerkBeheerder);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                LoadNetwork();
            }
            else
            {
                NetworkCanvas.Children.Remove(NetworkCanvas.Children[^1]);
            }

            isAddPointMode = false;
            this.Cursor = Cursors.Arrow;
        }
        private void MaakRoute_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEllipses.Count < 5)
            {
                MessageBox.Show("Selecteer minstens 5 punten om een route aan te maken.");
                return;
            }

            string routeNaam = Microsoft.VisualBasic.Interaction.InputBox(
                "Geef een naam voor de nieuwe route (min. 3 letters):",
                "Nieuwe Route",
                "Route");

            if (string.IsNullOrWhiteSpace(routeNaam) || routeNaam.Length < 3)
            {
                MessageBox.Show("Ongeldige naam. De naam moet minstens 3 karakters bevatten.");
                return;
            }

            try
            {
                List<NetworkPoint> punten = selectedEllipses
                    .Select(el => (int)el.Tag)
                    .Select(id =>
                    {
                        var uiPoint = points.FirstOrDefault(p => p.Id == id);
                        if (uiPoint == null)
                            throw new InvalidOperationException($"Punt met id {id} bestaat niet meer in de lijst.");
                        return NetworkPointMapper.MapToDomain(uiPoint);
                    })
                    .ToList();

                var routeBeheerder = new RouteBeheerder(new RouteRepository());
                routeBeheerder.MaakNieuweRoute(routeNaam, punten);

                foreach (var ellipse in selectedEllipses)
                {
                    ellipse.Fill = Brushes.Red;
                    ellipse.Width = 5;
                    ellipse.Height = 5;
                }
                selectedEllipses.Clear();

                MessageBox.Show("Route succesvol aangemaakt.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het aanmaken van de route: " + ex.Message);
            }

        }
        private void ShowRoute_Click(object sender, RoutedEventArgs e)
        {
            var routeBeheerder = new RouteBeheerder(new RouteRepository());
            var alleRoutes = routeBeheerder.GetAllRoutes();

            var routeNamen = alleRoutes.Select(r => r.Naam).ToList();
            string gekozenNaam = Microsoft.VisualBasic.Interaction.InputBox(
                "Kies een route:\n" + string.Join("\n", routeNamen),
                "Route tonen",
                routeNamen.FirstOrDefault() ?? "");

            if (string.IsNullOrWhiteSpace(gekozenNaam))
                return;

            var gekozenRoute = alleRoutes.FirstOrDefault(r => r.Naam.Equals(gekozenNaam, StringComparison.OrdinalIgnoreCase));
            if (gekozenRoute == null)
            {
                MessageBox.Show("Route niet gevonden.");
                return;
            }

            ToonRouteOpCanvas(gekozenRoute);
        }
        private void ToonRouteOpCanvas(Route route)
        {
            foreach (var ellipse in pointEllipses.Values)
            {
                ellipse.Fill = Brushes.Red;
                ellipse.Width = 5;
                ellipse.Height = 5;
            }
            selectedEllipses.Clear();
            routeEllipses.Clear();

            foreach (var punt in route.Punten)
            {
                if (pointEllipses.TryGetValue(punt.Id, out var ellipse))
                {
                    ellipse.Fill = punt.IsStopPlaats ? Brushes.Orange : Brushes.Blue;
                    ellipse.Width = 7;
                    ellipse.Height = 7;
                    selectedEllipses.Add(ellipse);
                    routeEllipses.Add(ellipse);
                }
            }
            isRouteShown = true;
        }
        private void StopRouteTonen_Click(object sender, RoutedEventArgs e)
        {
            // Reset alle ellipsen naar standaard
            foreach (var ellipse in routeEllipses)
            {
                ellipse.Fill = Brushes.Red;
                ellipse.Width = 5;
                ellipse.Height = 5;
            }
            selectedEllipses.Clear();
            routeEllipses.Clear();
            isRouteShown = false;
        }
    }
}
