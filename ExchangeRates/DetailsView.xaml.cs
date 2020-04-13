using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace ExchangeRates
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class DetailsView : Page
    {
        public string Code { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset DateFrom { get; set; }
        public DateTimeOffset DateUntil { get; set; }
        private Cash cash;

        public DetailsView()
        {
            Currency = "example";
            Code = "example";
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Cash)
            {
                cash = (Cash)e.Parameter;
                Currency = cash.Currency;
                Code = cash.Code;
            }
            base.OnNavigatedTo(e);
        }

        private void BackToMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            DateFrom = DateFromPicker.Date.GetValueOrDefault();
            DateUntil = DateUntilPicker.Date.GetValueOrDefault();
            if (DatesAreCorrect())
            {
                string fromToRequest = DateHelper.getDateCorrectForApi(DateFrom);
                string untilToRequest = DateHelper.getDateCorrectForApi(DateUntil);
                ChangeChart(fromToRequest, untilToRequest);
            }
        }

        private async Task ChangeChart(string fromToRequest, string untilToRequest)
        {
            IList<Rate> dataToChart = await ApiRequestor.GetCurrencyFromTo(cash, fromToRequest, untilToRequest);
            ((LineSeries)LineChart.Series[0]).ItemsSource  = dataToChart;
        }

        private bool DatesAreCorrect()
        {
            if(DateFrom == null || DateUntil == null)
            {
                return false;
            }
            if(DateFrom > DateUntil)
            {
                return false;
            }
            return true;
        }
    }
}
