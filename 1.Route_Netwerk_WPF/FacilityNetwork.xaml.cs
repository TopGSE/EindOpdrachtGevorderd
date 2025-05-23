using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using _1.Route_Netwerk_WPF.Dto;
using _1.Route_Netwerk_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace _1.Route_Netwerk_WPF
{
    public partial class FacilityNetwork : Window
    {
        private readonly NetwerkBeheerder beheerder;
        private readonly NetworkPointUI geselecteerdPunt;
        private ObservableCollection<Facility> alleFaciliteiten;
        private ObservableCollection<Facility> geselecteerdeFaciliteiten;

        public FacilityNetwork(NetwerkBeheerder beheerder, NetworkPointUI geselecteerdPunt)
        {
            InitializeComponent();
            this.beheerder = beheerder ?? throw new ArgumentNullException(nameof(beheerder));
            this.geselecteerdPunt = geselecteerdPunt ?? throw new ArgumentNullException(nameof(geselecteerdPunt));

            LaadFaciliteiten();
        }

        private void LaadFaciliteiten()
        {
            try
            {
                var alle = beheerder.GetAllFacilities();
                var gekoppeld = beheerder.GeefFaciliteitenVoorPoint(NetworkPointMapper.MapToDomain(geselecteerdPunt));

                geselecteerdeFaciliteiten = new ObservableCollection<Facility>(gekoppeld);
                var resterend = alle.Where(f => !gekoppeld.Any(g => g.Id == f.Id)).ToList();
                alleFaciliteiten = new ObservableCollection<Facility>(resterend);

                ListBoxAlleFaciliteiten.ItemsSource = alleFaciliteiten;
                ListBoxGeselecteerdeFaciliteiten.ItemsSource = geselecteerdeFaciliteiten;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden faciliteiten: {ex.Message}");
            }
        }

        private void VoegFaciliteitToe_Click(object sender, RoutedEventArgs e)
        {
            var selectie = ListBoxAlleFaciliteiten.SelectedItems.Cast<Facility>().ToList();
            foreach (var f in selectie)
            {
                geselecteerdeFaciliteiten.Add(f);
                alleFaciliteiten.Remove(f);
            }
        }

        private void VerwijderFaciliteit_Click(object sender, RoutedEventArgs e)
        {
            var selectie = ListBoxGeselecteerdeFaciliteiten.SelectedItems.Cast<Facility>().ToList();
            foreach (var f in selectie)
            {
                alleFaciliteiten.Add(f);
                geselecteerdeFaciliteiten.Remove(f);
            }
        }

        private void VoegAlleFaciliteitenToe_Click(object sender, RoutedEventArgs e)
        {
            foreach (var f in alleFaciliteiten)
            {
                geselecteerdeFaciliteiten.Add(f);
            }
            alleFaciliteiten.Clear();
        }

        private void VerwijderAlleFaciliteiten_Click(object sender, RoutedEventArgs e)
        {
            foreach (var f in geselecteerdeFaciliteiten)
            {
                alleFaciliteiten.Add(f);
            }
            geselecteerdeFaciliteiten.Clear();
        }

        private void VoegFaciliteitenToe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var domeinPunt = NetworkPointMapper.MapToDomain(geselecteerdPunt);
                beheerder.StelFaciliteitenInVoorPoint(domeinPunt, geselecteerdeFaciliteiten.ToList());
                MessageBox.Show("Faciliteiten succesvol gekoppeld.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}");
            }
        }
    }
}
