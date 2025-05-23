using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Route_Netwerk_WPF.Models
{
    public class FacilityUI : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        public FacilityUI()
        {
        }

        public FacilityUI(string name)
        {
            Name = name;
        }

        public FacilityUI(int id, string name)
        {
            Id = id;
            Name = name;
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
        public string Name 
        { 
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        } 

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }
    }
}
