﻿using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RouteWPF
{
    /// <summary>
    /// Interaction logic for RouteWindow.xaml
    /// </summary>
    public partial class RouteWindow : Window
    {
        private readonly RouteBeheerder routeBeheerder;
        private readonly NetwerkBeheerder netwerkBeheerder = new NetwerkBeheerder(new NetwerkRepository());
        private readonly int routeId;
        private Route loadedRoute;

        public RouteWindow(int id)
        {
            InitializeComponent();
            routeId = id;
            routeBeheerder = new RouteBeheerder(new RouteRepository());
            LoadGegevens();
            //for safety commit added line
        }

        private void LoadGegevens()
        {
            try
            {
                loadedRoute = routeBeheerder.GetRouteById(routeId);
                DataContext = loadedRoute;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het laden van de route: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update the route name from the TextBox (in case user changed it)
                loadedRoute.Naam = NaamTextBox.Text;

                // The DataGrid is bound to loadedRoute.Punten directly, so IsStopPlaats is updated automatically via binding.

                // Call the update method
                routeBeheerder.UpdateRoute(loadedRoute);

                MessageBox.Show("Route succesvol opgeslagen!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het opslaan van de route: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VoegPuntInBegin_Click(object sender, RoutedEventArgs e)
        {
            VoegPuntAanRouteToe(inBegin: true);
        }

        private void VoegPuntOpEinde_Click(object sender, RoutedEventArgs e)
        {
            VoegPuntAanRouteToe(inBegin: false);
        }

        private void VoegPuntAanRouteToe(bool inBegin)
        {
            // Vraag direct om een ID
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Geef het ID van het netwerkpunt dat je wilt toevoegen:",
                "Punt toevoegen");

            if (!int.TryParse(input, out int gekozenId))
            {
                MessageBox.Show("Ongeldig ID.");
                return;
            }

            var allePoints = netwerkBeheerder.GetNetworkPoints();
            var bestaandeIds = loadedRoute.Punten.Select(p => p.Id).ToHashSet();
            var gekozenPunt = allePoints.FirstOrDefault(p => p.Id == gekozenId);

            if (gekozenPunt == null)
            {
                MessageBox.Show("Dit netwerkpunt bestaat niet.");
                return;
            }
            if (bestaandeIds.Contains(gekozenId))
            {
                MessageBox.Show("Dit punt zit al in de route.");
                return;
            }

            if (inBegin)
                loadedRoute.Punten.Insert(0, gekozenPunt);
            else
                loadedRoute.Punten.Add(gekozenPunt);

            PointsDataGrid.Items.Refresh();
        }



    }
}
