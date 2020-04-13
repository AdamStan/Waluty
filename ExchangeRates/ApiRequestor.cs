using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;
using System.Diagnostics;

namespace ExchangeRates
{
    public class ApiRequestor
    {
        static HttpClient client = new HttpClient();
        private readonly static string BaseUrl = "http://api.nbp.pl/api/exchangerates/tables/";
        private readonly static string BaseUrlToGetFlag = "https://restcountries.eu/rest/v2/currency/";
        // one currency: 
        // http://api.nbp.pl/api/exchangerates/rates/{table}/{code}/{startDate}/{endDate}/
        // example: http://api.nbp.pl/api/exchangerates/rates/A/USD/2019-01-01/2019-01-30/
        private readonly static string BaseUrlOneCurrency = "http://api.nbp.pl/api/exchangerates/rates/";

        // flag
        private readonly static string BaseFlagAddress = "https://www.countryflags.io/";
        private readonly static string RestFlagAddress = "/flat/24.png";

        public static async Task<string> GetJsonAsync(string path)
        {
            string content = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            return content;
        }

        /// <summary>
        /// Get currencies from NBP API
        /// </summary>
        /// <param name="table"> name of table to request (from api) </param>
        /// <param name="date"> if null then we take current exchange rates </param> 
        /// <returns></returns>
        public static async Task<IList<Cash>> GetAllCashAsync(string table, string date = "")
        {
            List<Cash> currencies = new List<Cash>();
            
            string content = await GetJsonAsync(BaseUrl + table + "/" + date + "/");
            if (content.Equals("404 NotFound - Not Found - Brak danych"))
            {
                Debug.WriteLine("Data not found!");
                return currencies;
            }

            currencies.AddRange(JsonParserForCurrencies.GetCashFromJson(content));

            currencies.ForEach(async cur => {
                try
                {
                    string pathToImage = await GetFlag(cur);
                    cur.TableName = table;
                    cur.PathToImage = pathToImage;
                } catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            });

            return currencies;
        }

        internal static async Task<IList<Rate>> GetCurrencyFromTo(Cash cash, string fromToRequest, string untilToRequest)
        {
            string table = cash.TableName;
            string code = cash.Code;
            string urlAddress = BaseUrlOneCurrency + table + "/" + code + "/" + fromToRequest + "/" + untilToRequest + "/";
            Debug.WriteLine("Url address: " + urlAddress);
            string jsonWithData = await GetJsonAsync(urlAddress);
            Debug.WriteLine("Rates in json: " + jsonWithData);
            return JsonParserForCurrencies.GetPriceFromJson(jsonWithData);
        }

        public static async Task<string> GetFlag(Cash cash)
        {
            string jsonWithImage = await GetJsonAsync(BaseUrlToGetFlag + cash.Code);
            return BaseFlagAddress + JsonParserForCurrencies.GetAlpha2CodeUrlFromJson(jsonWithImage) + RestFlagAddress;
        }

    }

    class JsonParserForCurrencies
    {
        public static IList<Cash> GetCashFromJson(string json)
        {
            IList<Cash> currencies = new List<Cash>();
            var jsonContent = JsonArray.Parse(json);
            foreach (var table in jsonContent)
            {
                var jsonObject = table.GetObject();
                var date = jsonObject.GetNamedString("effectiveDate");
                foreach (var currency in jsonObject.GetNamedArray("rates"))
                {
                    var currencyObject = currency.GetObject();
                    currencies.Add(new Cash() {
                        Currency = currencyObject.GetNamedString("currency"),
                        Code = currencyObject.GetNamedString("code"),
                        Mid = currencyObject.GetNamedNumber("mid"),
                        EffectiveDate = Convert.ToDateTime(date),
                    });
                }
            }
            return currencies;
        }
        
        public static string GetAlpha2CodeUrlFromJson(string json)
        {
            string imageUrl = "";
            if (JsonArray.TryParse(json, out JsonArray jsonContentArray))
            {
                foreach (var table in jsonContentArray)
                {
                    var jsonObject = table.GetObject();
                    imageUrl = jsonObject.GetNamedString("alpha2Code");
                    break;
                }
            }
            return imageUrl;
        }

        public static IList<Rate> GetPriceFromJson(string json)
        {
            IList<Rate> rates = new List<Rate>();
            var jsonObject = JsonObject.Parse(json);
            var jsonArray = jsonObject.GetNamedArray("rates");
            foreach (var jsonRate in jsonArray)
            {
                var rate = jsonRate.GetObject();
                string effectiveDate = rate.GetNamedString("effectiveDate");
                double mid = rate.GetNamedNumber("mid");
                Rate newRate = new Rate(effectiveDate, mid);
                rates.Add(newRate);
            }
            return rates;
        }
    }
}
