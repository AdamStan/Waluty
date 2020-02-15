using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    /**
     * Class for Currency parameters, more at http://api.nbp.pl
     */
    class Cash
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
    }
}
