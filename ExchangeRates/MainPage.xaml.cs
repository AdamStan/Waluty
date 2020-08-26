using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace ExchangeRates
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly StorageManager storageManager = new StorageManager();
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
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
        }

        public async void InitializeTable(string tableName, string date = "")
        {
            try
            {
                List<Cash> currencies = await ApiRequestor.GetAllCashAsync(tableName, date);
                ApiRequestor.GetFlagsForCurrencies(currencies);
                this.ViewModel.AddCurrencies(currencies);
            }
            catch (Exception)
            {
                Debug.WriteLine("Cannot get data for: " + tableName);
            }
        }

        internal void Initialize(string date = "")
        {
            ObservableCollection<Cash> currencies = storageManager.GetRatesFromStorage();
            if (!date.Equals("") || currencies.Count == 0)
            {
                InitializeTable("A", date);
                InitializeTable("B", date);
            }
            else 
            {
                this.ViewModel.AddCurrencies(currencies);
            }
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
        }

        public void ListViewUpdated()
        {
            // binding sometimes doesn't work
            CurrenciesListView.ItemsSource = ViewModel.ManyCash;
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
                UpdateStorage();
            }
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void App_Resuming(Object sender, Object e)
        {
            // TODO: implement
            Debug.WriteLine("Application is resuming!!!");
        }

        public void UpdateStorage()
        {
            Debug.WriteLine("Update storage");
            storageManager.SetRatesToStorage(this.ViewModel);
        }

        public static void UpdateStorage(Cash cash)
        {
            Debug.WriteLine("Update storage - cash");
            storageManager.SetRatesToStorage(cash);
        }

        private void Refresh_Ignore_Cache(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("clicked");
            storageManager.ClearLocalStorage();
            Initialize();
        }
    }
}
