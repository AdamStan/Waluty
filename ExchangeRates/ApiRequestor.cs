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
        private readonly static string AllLastCurrenciesA = "A/last/1/";
        private readonly static string AllLastCurrenciesB = "B/last/1";
        private readonly static string BaseUrlToGetFlag = "https://restcountries.eu/rest/v2/currency/";

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
            // string contentA = await GetJsonAsync(BaseUrl + AllLastCurrenciesA);
            // string contentB = await GetJsonAsync(BaseUrl + AllLastCurrenciesB);
            string contZznakami = "[{\"table\":\"A\",\"no\":\"032/A/NBP/2020\",\"effectiveDate\":\"2020-02-17\",\"rates\":[{\"currency\":\"bat (Tajlandia)\",\"code\":\"THB\",\"mid\":0.1258},{\"currency\":\"dolar amerykański\",\"code\":\"USD\",\"mid\":3.9189},{\"currency\":\"dolar australijski\",\"code\":\"AUD\",\"mid\":2.6364},{\"currency\":\"dolar Hongkongu\",\"code\":\"HKD\",\"mid\":0.5046},{\"currency\":\"dolar kanadyjski\",\"code\":\"CAD\",\"mid\":2.9623},{\"currency\":\"dolar nowozelandzki\",\"code\":\"NZD\",\"mid\":2.5224},{\"currency\":\"dolar singapurski\",\"code\":\"SGD\",\"mid\":2.8216},{\"currency\":\"euro\",\"code\":\"EUR\",\"mid\":4.2502},{\"currency\":\"forint (Węgry)\",\"code\":\"HUF\",\"mid\":0.012699},{\"currency\":\"frank szwajcarski\",\"code\":\"CHF\",\"mid\":3.9921},{\"currency\":\"funt szterling\",\"code\":\"GBP\",\"mid\":5.1109},{\"currency\":\"hrywna (Ukraina)\",\"code\":\"UAH\",\"mid\":0.1603},{\"currency\":\"jen (Japonia)\",\"code\":\"JPY\",\"mid\":0.035664},{\"currency\":\"korona czeska\",\"code\":\"CZK\",\"mid\":0.1715},{\"currency\":\"korona duńska\",\"code\":\"DKK\",\"mid\":0.5689},{\"currency\":\"korona islandzka\",\"code\":\"ISK\",\"mid\":0.030911},{\"currency\":\"korona norweska\",\"code\":\"NOK\",\"mid\":0.4233},{\"currency\":\"korona szwedzka\",\"code\":\"SEK\",\"mid\":0.4034},{\"currency\":\"kuna (Chorwacja)\",\"code\":\"HRK\",\"mid\":0.5705},{\"currency\":\"lej rumuński\",\"code\":\"RON\",\"mid\":0.8901},{\"currency\":\"lew (Bułgaria)\",\"code\":\"BGN\",\"mid\":2.1731},{\"currency\":\"lira turecka\",\"code\":\"TRY\",\"mid\":0.6485},{\"currency\":\"nowy izraelski szekel\",\"code\":\"ILS\",\"mid\":1.1424},{\"currency\":\"peso chilijskie\",\"code\":\"CLP\",\"mid\":0.004949},{\"currency\":\"peso filipińskie\",\"code\":\"PHP\",\"mid\":0.0775},{\"currency\":\"peso meksykańskie\",\"code\":\"MXN\",\"mid\":0.2111},{\"currency\":\"rand (Republika Południowej Afryki)\",\"code\":\"ZAR\",\"mid\":0.2631},{\"currency\":\"real (Brazylia)\",\"code\":\"BRL\",\"mid\":0.9117},{\"currency\":\"ringgit (Malezja)\",\"code\":\"MYR\",\"mid\":0.9460},{\"currency\":\"rubel rosyjski\",\"code\":\"RUB\",\"mid\":0.0619},{\"currency\":\"rupia indonezyjska\",\"code\":\"IDR\",\"mid\":0.00028704},{\"currency\":\"rupia indyjska\",\"code\":\"INR\",\"mid\":0.05487},{\"currency\":\"won południowokoreański\",\"code\":\"KRW\",\"mid\":0.003311},{\"currency\":\"yuan renminbi (Chiny)\",\"code\":\"CNY\",\"mid\":0.5615},{\"currency\":\"SDR (MFW)\",\"code\":\"XDR\",\"mid\":5.3608}]}]";
            // currencies.AddRange(JsonParserForCurrencies.GetCashFromJson(contentA));
            currencies.AddRange(JsonParserForCurrencies.GetCashFromJson(contZznakami));
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
            //throw new Exception(jsonWithImage);
            //string jsonWithImage = "[{\"name\":\"Romania\",\"topLevelDomain\":[\".ro\"],\"alpha2Code\":\"RO\",\"alpha3Code\":\"ROU\",\"callingCodes\":[\"40\"],\"capital\":\"Bucharest\",\"altSpellings\":[\"RO\",\"Rumania\",\"Roumania\",\"România\"],\"region\":\"Europe\",\"subregion\":\"Eastern Europe\",\"population\":19861408,\"latlng\":[46.0,25.0],\"demonym\":\"Romanian\",\"area\":238391.0,\"gini\":30.0,\"timezones\":[\"UTC + 02:00\"],\"borders\":[\"BGR\",\"HUN\",\"MDA\",\"SRB\",\"UKR\"],\"nativeName\":\"România\",\"numericCode\":\"642\",\"currencies\":[{\"code\":\"RON\",\"name\":\"Romanian leu\",\"symbol\":\"lei\"}],\"languages\":[{\"iso639_1\":\"ro\",\"iso639_2\":\"ron\",\"name\":\"Romanian\",\"nativeName\":\"Română\"}],\"translations\":{\"de\":\"Rumänien\",\"es\":\"Rumania\",\"fr\":\"Roumanie\",\"ja\":\"ルーマニア\",\"it\":\"Romania\",\"br\":\"Romênia\",\"pt\":\"Roménia\",\"nl\":\"Roemenië\",\"hr\":\"Rumunjska\",\"fa\":\"رومانی\"},\"flag\":\"https://restcountries.eu/data/rou.svg\",\"regionalBlocs\":[{\"acronym\":\"EU\",\"name\":\"European Union\",\"otherAcronyms\":[],\"otherNames\":[]}],\"cioc\":\"ROU\"}]";
            return JsonParserForCurrencies.GetFlagUrlFromJson(jsonWithImage);
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
        
        public static string GetFlagUrlFromJson(string json)
        {
            string imageUrl = "";
            JsonArray jsonContentArray;
            if (JsonArray.TryParse(json, out jsonContentArray)) {
                foreach (var table in jsonContentArray)
                {
                    var jsonObject = table.GetObject();
                    imageUrl = jsonObject.GetNamedString("flag");
                    break;
                }
            }
            return imageUrl;
        }
    }
}
