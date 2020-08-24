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
        private IList<Rate> dataToChart;

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

        private async void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            DateFrom = DateFromPicker.Date.GetValueOrDefault();
            DateUntil = DateUntilPicker.Date.GetValueOrDefault();
            if (DatesAreCorrect())
            {
                string fromToRequest = DateHelper.getDateCorrectForApi(DateFrom);
                string untilToRequest = DateHelper.getDateCorrectForApi(DateUntil);
                await ChangeChart(fromToRequest, untilToRequest);
            }
        }

        private async Task ChangeChart(string fromToRequest, string untilToRequest)
        {
            dataToChart = await ApiRequestor.GetCurrencyFromTo(cash, fromToRequest, untilToRequest);
            LineSeries series = ((LineSeries)LineChart.Series[0]);
            series.ItemsSource  = dataToChart;
            ExportButton.IsEnabled = true;
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
            if(DateUntil > DateTime.Now)
            {
                return false;
            }
            return true;
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("comma-separated values", new List<string>() { ".csv" });
            savePicker.SuggestedFileName = "new";
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                string content = CreateCSVData();
                await Windows.Storage.FileIO.WriteTextAsync(file, content);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Debug.Write("File " + file.Name + " was saved.");
                }
                else
                {
                    Debug.Write("File " + file.Name + " couldn't be saved.");
                }
            }
            else
            {
                Debug.Write("Operation cancelled");
            }
        }

        private string CreateCSVData()
        {
            string csv = Currency;
            foreach (Rate rate in dataToChart)
            {
                csv += "\r\n";
                string line = string.Format("{0};{1}", rate.Mid, rate.Date);
                csv += line;
            }
            return csv;
        }
    }
}
