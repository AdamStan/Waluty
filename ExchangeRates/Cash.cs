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
        public string Code { get; set; }
        public DateTime EffectiveDate { get; set; }
        /** przeliczony kurs średni waluty */
        public double Mid { get; set; }
        public Cash()
        {
            this.Currency = "Złoty";
            this.Code = "ZLN";
        }
        public string DateWithoutHour
        {
            get
            {
                return $"{EffectiveDate.Day}/{EffectiveDate.Month}/{EffectiveDate.Year}";
            }
        }
    }

    public class CashViewModel
    {
        private ObservableCollection<Cash> manyCash = new ObservableCollection<Cash>();
        public ObservableCollection<Cash> ManyCash { get { return this.manyCash; } }

        internal void AddCurrencies(IList<Cash> currencies)
        {
            foreach(Cash currency in currencies)
            {
                manyCash.Add(currency);
            }
        }

        public CashViewModel()
        {
        }
    }
}
