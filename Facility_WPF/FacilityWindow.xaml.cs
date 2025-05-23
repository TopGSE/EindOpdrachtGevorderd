using _1.Route_Netwerk_WPF.Models;
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

namespace Facility_WPF
{
    /// <summary>
    /// Interaction logic for FacilityWindow.xaml
    /// </summary>
    public partial class FacilityWindow : Window
    {
        bool isUpdate;
        public FacilityUI Facility;

        public FacilityWindow(bool isUpdate, FacilityUI facility)
        {
            this.isUpdate = isUpdate;
            InitializeComponent();
            if (isUpdate)
            {
                IdTextBox.Text = facility.Id.ToString();
                NameTextBox.Text = facility.Name;
                SaveButton.Content = "Update";
                Facility = facility;
            }
            else
            {
                SaveButton.Content = "Add";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(isUpdate)
            {
                Facility.Id = int.Parse(IdTextBox.Text);
                Facility.Name = NameTextBox.Text;
            }
            else
            {
                Facility = new FacilityUI
                {
                    Name = NameTextBox.Text
                };
            }
            DialogResult = true;
            Close();
        }
    }
}
