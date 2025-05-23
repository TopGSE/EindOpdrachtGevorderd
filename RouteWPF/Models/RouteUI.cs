using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteWPF.Models
{
    public class RouteUI : INotifyPropertyChanged
    {
        private int _id;
        private string _naam = string.Empty;
        private List<NetworkPoint> _punten = new();

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Naam
        {
            get => _naam;
            set
            {
                if (_naam != value)
                {
                    _naam = value;
                    OnPropertyChanged(nameof(Naam));
                }
            }
        }

        public List<NetworkPoint> Punten
        {
            get => _punten;
            set
            {
                if (_punten != value)
                {
                    _punten = value;
                    OnPropertyChanged(nameof(Punten));
                }
            }
        }

        public RouteUI(string naam)
        {
            Naam = naam;
        }

        public RouteUI(int id, string naam)
        {
            Id = id;
            Naam = naam;
        }

        public RouteUI(string naam, List<NetworkPoint> punten)
        {
            Naam = naam;
            Punten = punten;
        }

        public RouteUI(int id, string naam, List<NetworkPoint> punten)
        {
            Id = id;
            Naam = naam;
            Punten = punten;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
