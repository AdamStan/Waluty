using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ExchangeRates
{
    /**
     * Class for Currency parameters, more at http://api.nbp.pl
     */
    public class Cash : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        /** Name */
        public string Currency { get; set; }
        public string Code { get; set; }
        public DateTime EffectiveDate { get; set; }
        /** przeliczony kurs średni waluty */
        public double Mid { get; set; }

        private string pathToImage;
        public string PathToImage
        {
            get
            {
                return pathToImage;
            }
            set
            {
                this.pathToImage = value;
                this.OnPropertyChanged();
            }
        }

        public string DateWithoutHour
        {
            get
            {
                return $"{EffectiveDate.Day}/{EffectiveDate.Month}/{EffectiveDate.Year}";
            }
        }

        public string MidWithDescription
        {
            get
            {
                return Mid + " PLN";
            }
        }

        public Cash()
        {
            pathToImage = "";
        }

        public override string ToString()
        {
            return pathToImage;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CashViewModel
    {
        private ObservableCollection<Cash> manyCash = new ObservableCollection<Cash>();

        public ObservableCollection<Cash> ManyCash { get { return this.manyCash; } }

        public CashViewModel()
        {
        }

        internal void AddCurrencies(IList<Cash> currencies)
        {
            foreach (Cash currency in currencies)
            {
                manyCash.Add(currency);
            }
        }
    }
}
