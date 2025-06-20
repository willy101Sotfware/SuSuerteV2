using System.ComponentModel;

namespace SuSuerteV2.Domain.ApiService.Models
{
    public class ValueModel : INotifyPropertyChanged
    {
        private decimal _Val;
        public decimal Val
        {
            get { return _Val; }
            set
            {
                _Val = value;
                NotifyPropertyChanged("Val");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
