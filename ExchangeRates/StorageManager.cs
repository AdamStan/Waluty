using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ExchangeRates
{
    class StorageManager
    {
        private const char SEPARATOR = '<';
        private static readonly CultureInfo DOUBLE_FORMAT = new CultureInfo("en-US");
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public void SetRatesToStorage(CashViewModel model)
        {
            ApplicationDataCompositeValue currentCurrencies = new ApplicationDataCompositeValue();
            foreach (var cur in model.ManyCash)
            {
                string toWrite = cur.Currency
                    + SEPARATOR + cur.Mid.ToString(DOUBLE_FORMAT)
                    + SEPARATOR + cur.DateWithoutHour + SEPARATOR + cur.TableName + 
                    SEPARATOR + cur.PathToImage;
                currentCurrencies[cur.Code] = toWrite;
            }
            localSettings.Values["currencies"] = currentCurrencies;
            localSettings.Values["day"] = DateTime.Now.Day;
            localSettings.Values["month"] = DateTime.Now.Month;
            localSettings.Values["year"] = DateTime.Now.Year;
        }

        public ObservableCollection<Cash> GetRatesFromStorage()
        {
            ObservableCollection<Cash> currencies = new ObservableCollection<Cash>();
            ApplicationDataCompositeValue oldCurrencies = (ApplicationDataCompositeValue)localSettings.Values["currencies"];
            DateTime dateWhenLastSaveWasDone = GetDateWhenLastStorageWasSaved();
            if (oldCurrencies != null && dateWhenLastSaveWasDone.CompareTo(DateTime.Now.Date) == 0)
            {
                foreach (KeyValuePair<string, object> entry in oldCurrencies)
                {
                    string code = entry.Key;
                    string restValues = (string)entry.Value;
                    string[] values = restValues.Split(SEPARATOR);
                    Cash cash = new Cash();
                    cash.Code = code;
                    cash.Currency = values[0];
                    cash.Mid = Convert.ToDouble(values[1], DOUBLE_FORMAT);
                    string[] dateValues = values[2].Split(Cash.DATE_SEPARATOR);
                    cash.EffectiveDate = new DateTime(Convert.ToInt32(dateValues[2]), Convert.ToInt32(dateValues[1]), Convert.ToInt32(dateValues[0]));
                    cash.TableName = values[3];
                    cash.PathToImage = values[4];
                    currencies.Add(cash);
                }
            }
            return currencies;
        }

        private DateTime GetDateWhenLastStorageWasSaved()
        {
            int day = Convert.ToInt32(localSettings.Values["day"]);
            int month = Convert.ToInt32(localSettings.Values["month"]);
            int year = Convert.ToInt32(localSettings.Values["year"]);
            if (day == 0 || month == 0 || year == 0)
            {
                return DateTime.Now.Date;
            }
            return new DateTime(year, month, day);
        }

        internal void SetRatesToStorage(Cash cash)
        {
            ApplicationDataCompositeValue currentCurrencies = (ApplicationDataCompositeValue)localSettings.Values["currencies"];
            if (currentCurrencies == null)
            {
                currentCurrencies = new ApplicationDataCompositeValue();
            }
            string toWrite = cash.Currency
                    + SEPARATOR + cash.Mid.ToString(DOUBLE_FORMAT)
                    + SEPARATOR + cash.DateWithoutHour + SEPARATOR + cash.TableName +
                    SEPARATOR + cash.PathToImage;
            Debug.WriteLine(toWrite);
            currentCurrencies[cash.Code] = toWrite;
            localSettings.Values["currencies"] = currentCurrencies;
        }

        internal void ClearLocalStorage()
        {
            localSettings.Values["currencies"] = null;
            localSettings.Values["day"] = null;
            localSettings.Values["month"] = null;
            localSettings.Values["year"] = null;
        }
    }
}
