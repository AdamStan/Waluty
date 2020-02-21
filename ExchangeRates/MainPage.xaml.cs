﻿using System;
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

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace ExchangeRates
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public DateTime Date;

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = new CashViewModel();
            this.Date = DateTime.Now;
            Initialize();
        }
        
        public async void Initialize()
        {
            IList<Cash> currencies = await ApiRequestor.GetAllCashAsync();
            this.ViewModel.AddCurrencies(currencies);
        }

        public CashViewModel ViewModel { get; set; }

        // https://docs.microsoft.com/en-us/windows/uwp/design/basics/navigate-between-two-pages
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            object clickedItem = e.ClickedItem;
            Cash cash = (Cash) clickedItem;
            System.Diagnostics.Debug.WriteLine(sender);
            this.Frame.Navigate(typeof(DetailsView), cash) ;
        }
    }
}
