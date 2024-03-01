using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveClient.Models
{
    class Backup : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        public string Type { get; set; }

        private long _encryptionTime;
        public long EncryptTime
        {

            get { return _encryptionTime; }
            set
            {
                if (_encryptionTime != value)
                {
                    _encryptionTime = value;
                }
            }
        }
        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private double _progression;
        public double Progression
        {
            get { return _progression; }
            set
            {
                if (_progression != value)
                {
                    _progression = value;
                    OnPropertyChanged(nameof(Progression));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
