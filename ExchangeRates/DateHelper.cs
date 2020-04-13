using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    class DateHelper
    {
        private DateHelper() { }

        public static string getDateCorrectForApi(DateTimeOffset datetime)
        {
            string date = "";
            date += datetime.Year + "-";
            if (datetime.Month < 10)
            {
                date += "0" + datetime.Month;
            }
            else
            {
                date += datetime.Month;
            }
            date += "-";
            if (datetime.Day < 10)
            {
                date += "0" + datetime.Day;
            }
            else
            {
                date += datetime.Day;
            }
            return date;
        }

    }
}
