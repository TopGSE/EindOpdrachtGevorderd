using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Route_Netwerk_WPF.Models
{
    public class SegmentUI : INotifyPropertyChanged
    {
        private int _id;
        private int _startPointId;
        private int _endPointId;
        public SegmentUI()
        {
        }

        public SegmentUI(int startPointId, int endPointId)
        {
            StartPointId = startPointId;
            EndPointId = endPointId;
        }

        public SegmentUI(int id, int startPointId, int endPointId)
        {
            Id = id;
            StartPointId = startPointId;
            EndPointId = endPointId;
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
        public int StartPointId 
        { 
            get { return _startPointId; }
            set
            {
                if (_startPointId != value)
                {
                    _startPointId = value;
                    OnPropertyChanged(nameof(StartPointId));
                }
            }
        }
        public int EndPointId 
        { 
            get { return _endPointId; }
            set
            {
                if (_endPointId != value)
                {
                    _endPointId = value;
                    OnPropertyChanged(nameof(EndPointId));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
