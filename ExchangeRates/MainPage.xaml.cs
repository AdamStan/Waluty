using System;
using System.Collections.Generic;
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
using System.Diagnostics;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace ExchangeRates
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string NoExchangeRateMessage
        {
            get
            {
                return "For this day is no exchange rates to show";
            }
        }
        public DateTime Date;

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = new CashViewModel(this);
            this.Date = DateTime.Now;
            Initialize();
        }

        public async void InitializeTable(string tableName, string date = "")
        {
            try
            {
                IList<Cash> currencies = await ApiRequestor.GetAllCashAsync(tableName, date);
                this.ViewModel.AddCurrencies(currencies);
            }
            catch (Exception)
            {
                Debug.WriteLine("Cannot get data for: " + tableName);
            }
        }

        internal void Initialize(string date = "")
        {
            InitializeTable("A", date);
            InitializeTable("B", date);
        }

        public CashViewModel ViewModel { get; set; }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            object clickedItem = e.ClickedItem;
            Cash cash = (Cash)clickedItem;
            Debug.WriteLine(sender);
            this.Frame.Navigate(typeof(DetailsView), cash);
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            DateTimeOffset toGet = e.NewDate;
            string month = "";
            if (toGet.Month < 10)
            {
                month = "0" + toGet.Month;
            }
            else
            {
                month = "" + toGet.Month;
            }
            string dateToGet = toGet.Year + "-" + month + "-" + toGet.Day + "/";
            ViewModel.RemoveAllCurrencies();
            Initialize(dateToGet);
            Debug.WriteLine(dateToGet);
            ListViewUpdated();
        }

        public void ListViewUpdated()
        {
            Debug.WriteLine(CurrenciesListView.Items.Count);
            if (CurrenciesListView.Items.Count == 0)
            {
                NoItemsTextBox.Visibility = Visibility.Visible;
                CurrenciesListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoItemsTextBox.Visibility = Visibility.Collapsed;
                CurrenciesListView.Visibility = Visibility.Visible;
            }
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
