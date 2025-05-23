using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace _1.Route_Netwerk_WPF
{
    public partial class RouteWindow : Window
    {
        private readonly RouteBeheerder routeBeheerder;
        private readonly List<NetworkPoint> geselecteerdePunten;

        public RouteWindow(RouteBeheerder routeBeheerder, List<NetworkPoint> punten)
        {
            InitializeComponent();
            this.routeBeheerder = routeBeheerder ?? throw new ArgumentNullException(nameof(routeBeheerder));
            geselecteerdePunten = punten ?? throw new ArgumentNullException(nameof(punten));
        }

        private void Bevestig_Click(object sender, RoutedEventArgs e)
        {
            string routeNaam = RouteNaamTextBox.Text.Trim();

            if (routeNaam.Length < 3)
            {
                MessageBox.Show("Route naam moet minstens 3 karakters bevatten.");
                return;
            }

            if (geselecteerdePunten.Count < 5)
            {
                MessageBox.Show("Je moet minstens 5 punten selecteren.");
                return;
            }

            try
            {
                routeBeheerder.MaakNieuweRoute(routeNaam, geselecteerdePunten);
                MessageBox.Show("Route succesvol aangemaakt.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij aanmaken route: " + ex.Message);
            }
        }
    }
}
