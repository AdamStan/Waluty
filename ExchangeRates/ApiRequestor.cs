using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace ExchangeRates
{
    public class ApiRequestor
    {
        static HttpClient client = new HttpClient();
        private readonly static string BaseUrl = "http://api.nbp.pl/api/exchangerates/tables/";
        private readonly static string AllLastCurrenciesA = "A/";
        private readonly static string AllLastCurrenciesB = "B/";
        private readonly static string BaseUrlToGetFlag = "https://restcountries.eu/rest/v2/currency/";
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
        
        public static async Task<IList<Cash>> GetAllCashAsync()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Cash> currencies = new List<Cash>();
            
            string contentA = await GetJsonAsync(BaseUrl + AllLastCurrenciesA);
            string contentB =  await GetJsonAsync(BaseUrl + AllLastCurrenciesB);
            currencies.AddRange(JsonParserForCurrencies.GetCashFromJson(contentA));
            currencies.AddRange(JsonParserForCurrencies.GetCashFromJson(contentB));

            currencies.ForEach(async cur => {
                try
                {
                    string pathToImage = await GetFlag(cur);
                    cur.PathToImage = pathToImage;
                    System.Diagnostics.Debug.WriteLine(pathToImage);
                } catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            });

            return currencies;
        }

        public static async Task<string> GetFlag(Cash cash)
        {
            System.Diagnostics.Debug.WriteLine(cash.Code);
            string jsonWithImage = await GetJsonAsync(BaseUrlToGetFlag + cash.Code);
            System.Diagnostics.Debug.WriteLine(jsonWithImage);
            return BaseFlagAddress + JsonParserForCurrencies.GetAlpha2CodeUrlFromJson(jsonWithImage) + RestFlagAddress;
        }

    }

    class JsonParserForCurrencies
    {
        public static IList<Cash> GetCashFromJson(string json)
        {
            IList<Cash> currencies = new List<Cash>();
            var jsonContentA = JsonArray.Parse(json);
            foreach (var table in jsonContentA)
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
                        EffectiveDate = Convert.ToDateTime(date)
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
    }
}
