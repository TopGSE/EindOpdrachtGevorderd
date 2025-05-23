using _1.Route_Netwerk_WPF.Dto;
using _1.Route_Netwerk_WPF.Models;
using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace _1.Route_Netwerk_WPF
{
    /// <summary>
    /// Interaction logic for NetworkWindow.xaml
    /// </summary>
    public partial class NetworkWindow : Window
    {
        private readonly NetwerkBeheerder beheerder;
        private readonly NetworkPointUI geselecteerdPunt;
        private List<Facility> faciliteiten;

        public NetworkWindow(NetworkPointUI geselecteerdPunt, NetwerkBeheerder beheerder)
        {
            InitializeComponent();
            this.geselecteerdPunt = geselecteerdPunt ?? throw new ArgumentNullException(nameof(geselecteerdPunt));
            this.beheerder = beheerder ?? throw new ArgumentNullException(nameof(beheerder));
            LaadPuntGegevens();
        }
        private void LaadPuntGegevens()
        {
            // Vul tekstvelden
            PointIdTextBox.Text = geselecteerdPunt.Id.ToString();
            XTextBox.Text = geselecteerdPunt.X.ToString();
            YTextBox.Text = geselecteerdPunt.Y.ToString();

            try
            {
                // Haal faciliteiten op
                faciliteiten = beheerder.GeefFaciliteitenVoorPoint(NetworkPointMapper.MapToDomain(geselecteerdPunt));
                FacilitiesComboBox.ItemsSource = faciliteiten;
                FacilitiesComboBox.DisplayMemberPath = "Naam";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden faciliteiten: {ex.Message}");
            }
        }
        private void EditFacility_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdPunt != null)
            {
                var fn = new FacilityNetwork(beheerder, geselecteerdPunt);
                bool? result = fn.ShowDialog();
                if (result == true)
                {
                    // Indien gewenst: herlaad faciliteiten
                    LaadPuntGegevens();
                }
            }
            else
            {
                MessageBox.Show("Selecteer een punt.");
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double newX = double.Parse(XTextBox.Text);
                double newY = double.Parse(YTextBox.Text);

                geselecteerdPunt.X = newX;
                geselecteerdPunt.Y = newY;

                // Eventueel: faciliteiten aanpassen/bijwerken in database hier

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
