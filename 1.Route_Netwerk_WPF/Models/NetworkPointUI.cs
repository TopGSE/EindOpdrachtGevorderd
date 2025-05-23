using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Route_Netwerk_WPF.Models
{
    public class NetworkPointUI : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private int _id;
        private List<Facility> _facilities = new();
        public NetworkPointUI()
        {
        }

        public NetworkPointUI(double x, double y)
        {
            X = x;
            Y = y;
        }

        public NetworkPointUI(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public NetworkPointUI(double x, double y, List<Facility> facilities) : this(x, y)
        {
            Facilities = facilities;
        }

        public NetworkPointUI(int id, double x, double y, List<Facility> facilities)
        {
            Id = id;
            X = x;
            Y = y;
            Facilities = facilities;
        }

        public int Id 
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }
        public double X 
        { 
            get { return _x; }
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
        public double Y 
        { 
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }
        public List<Facility> Facilities { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
