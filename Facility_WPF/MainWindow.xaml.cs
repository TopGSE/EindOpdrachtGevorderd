using _1.Route_Netwerk_WPF.Mappers;
using _1.Route_Netwerk_WPF.Models;
using _2.Route_Netwerk_BL.Interfaces;
using _2.Route_Netwerk_BL.Managers;
using _2.Route_Netwerk_BL.Models;
using _3.Route_Netwerk_DL;
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

namespace Facility_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private NetwerkBeheerder netwerkBeheerder;
    private ObservableCollection<FacilityUI> allFacilities;
    public MainWindow()
    {
        InitializeComponent();
        netwerkBeheerder = new NetwerkBeheerder(new NetwerkRepository());
        allFacilities = new ObservableCollection<FacilityUI>(netwerkBeheerder.GetAllFacilities().Select(FacilityMapper.MapToUI));
        FacilityDataGrid.ItemsSource = allFacilities;
    }
    private void AddFacility_Click(object sender, RoutedEventArgs e)
    {
        FacilityWindow facilityWindow = new FacilityWindow(false, null);
        bool? result = facilityWindow.ShowDialog();
        if (result == true)
        {
            try
            {
                Facility f = FacilityMapper.MapToDomain(facilityWindow.Facility);
                netwerkBeheerder.VoegFaciliteitToe(f);
                facilityWindow.Facility.Id = f.Id;
                allFacilities.Add(facilityWindow.Facility);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"een faciliteit met die naam bestaat al", ex.Message);
            }
        }
    }
    private void UpdateFacility_Click(object sender, RoutedEventArgs e)
    {
        FacilityWindow facilityWindow = new FacilityWindow(true, (FacilityUI)FacilityDataGrid.SelectedItem);
        bool? result = facilityWindow.ShowDialog();
        if (result == true)
        {
            try
            {
                netwerkBeheerder.UpdateFacility(FacilityMapper.MapToDomain((FacilityUI)FacilityDataGrid.SelectedItem));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"De faciliteit is gebonden aan een netwerkpunt", ex.Message);
            }
        }
    }
    private void VerwijderFacility_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (FacilityDataGrid.SelectedItem != null)
            {
                FacilityUI selectedFacility = (FacilityUI)FacilityDataGrid.SelectedItem;
                netwerkBeheerder.VerwijderFaciliteit(selectedFacility.Id);
                allFacilities.Remove(selectedFacility);
            }
        }
        catch (Exception ex) 
        {
            MessageBox.Show($"De faciliteit is gebonden aan een netwerkpunt", ex.Message);
        }
    }

}