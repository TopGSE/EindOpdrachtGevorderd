using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;
using RouteWPF.Mappers;
using RouteWPF.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RouteWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private RouteBeheerder routeBeheerder;
    public ObservableCollection<RouteUI> allRoutes; 

    public MainWindow()
    {
        InitializeComponent();
        routeBeheerder = new RouteBeheerder(new RouteRepository());
        allRoutes = new ObservableCollection<RouteUI>(routeBeheerder.GetAllRoutes().Select(RouteMapper.MapToUI));
        RoutesDataGrid.ItemsSource = allRoutes;
    }

    private void UpdateRoute_Click(object sender, RoutedEventArgs e)
    {
        if (RoutesDataGrid.SelectedItem is RouteUI selectedRoute)
        {
            RouteWindow rw = new RouteWindow(selectedRoute.Id);
            bool? result = rw.ShowDialog();
            if (result == true)
            {
                // Reload the updated route from the database
                var updatedRoute = routeBeheerder.GetRouteById(selectedRoute.Id);
                // Update the properties of the selected RouteUI
                selectedRoute.Naam = updatedRoute.Naam;
                // If you also want to update points, do:
                // selectedRoute.Punten = updatedRoute.Punten;
            }
        }
        else
        {
            MessageBox.Show("Selecteer een route om te bewerken.");
        }
    }


    private void DeleteRoute_Click(object sender, RoutedEventArgs e)
    {
        if (RoutesDataGrid.SelectedItem is RouteUI selectedRoute)
        {
            var result = MessageBox.Show("Weet je zeker dat je deze route wilt verwijderen?", "Bevestig Verwijderen", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                routeBeheerder.DeleteRoute(selectedRoute.Id);
                allRoutes.Remove(selectedRoute);
            }
        }
        else
        {
            MessageBox.Show("Selecteer een route om te verwijderen.");
        }

    }
}
