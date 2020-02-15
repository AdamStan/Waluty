using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    /**
     * Class for Currency parameters, more at http://api.nbp.pl
     */
    public class Cash
    {
        /** Name */
        public string Currency { get; set; }
        public string Country { get; set; }
        public DateTime EffectiveDate { get; set; }
        /** przeliczony kurs kupna waluty */
        public long Bid { get; set; }
        /** przeliczony kurs sprzedaży waluty */
        public long Ask { get; set; }
        /** przeliczony kurs średni waluty */
        public long Mid { get; set; }
        public Cash()
        {
            this.Currency = "Złoty";
            this.Country = "Poland";
        }
        public string Summary
        {
            get
            {
                return $"{this.Currency} by {this.Country}, released: ";
            }
        }
    }
    public class CashViewModel
    {
        private readonly Cash defaultCash = new Cash();
        public Cash DefaultCashing { get { return this.defaultCash;  } }
        private ObservableCollection<Cash> manyCash = new ObservableCollection<Cash>();
        public ObservableCollection<Cash> ManyCash { get { return this.manyCash; } }
        public CashViewModel()
        {
            this.manyCash.Add(new Cash() { Country = "European Union", Currency = "EUR" });
            this.manyCash.Add(new Cash() { Country = "Denmark", Currency = "DKK" });
            this.manyCash.Add(new Cash() { Country = "Croatia", Currency = "HRK" });
        }
    }
}
