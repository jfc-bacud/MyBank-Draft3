using MaterialDesignThemes.Wpf;
using MyBank_Draft3.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyBank_Draft3.Pages.Customer
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        Database _localdb;
        string localUID;
        string currencyUsed;

        public SettingsPage(string UID)
        {
            InitializeComponent();
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            _localdb = new Database();
            ViewAccount();
        }

        private void AccountBTN_Click(object sender, RoutedEventArgs e)
        {
            ViewAccount();
        }

        private void ViewAccount()
        {
            AccountView.Visibility = Visibility.Visible;
            GeneralView.Visibility = Visibility.Hidden;

            var customer = (from c in _localdb.db.Customers
                            where c.Customer_ID == localUID
                            select c).FirstOrDefault();

            userTB.Text = customer.Customer_Email;
            FirstTB.Text = customer.Customer_FirstName;
            LastTB.Text = customer.Customer_LastName;
            EmailTB.Text = customer.Customer_Email;
            PasswordTB.Text = customer.Customer_Password;
        }

        private void GeneralBTN_Click(object sender, RoutedEventArgs e)
        {
            AccountView.Visibility = Visibility.Visible;
            GeneralView.Visibility = Visibility.Hidden;

            var userWallet = (from c in _localdb.db.Customers
                              where c.User_ID == localUID
                              join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                              select u).FirstOrDefault();

            PopulateCB();
            RetrieveWallet();

            WalletAmountTB.Text = userWallet.UserWallet_Balance.ToString();
            CurrencyCB.SelectedItem = userWallet.UserWallet_Currency;


        }

        private void PopulateCB()
        {
            List<string> list = new List<string> { "USD", "EUR", "PHP", "YEN", "WON", "GBP" };
            CurrencyCB.ItemsSource = list;
        }


        private void RetrieveWallet()
        {
            var userWallet = (from c in _localdb.db.Customers
                              where c.User_ID == localUID
                              join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                              select u).FirstOrDefault();

            RetrieveCurrency(userWallet);

            BalanceTB.Text = currencyUsed + userWallet.UserWallet_Balance.ToString();

            RetrieveCurrencyIcon(userWallet.UserWallet_Currency.ToString());
        }

        private void RetrieveCurrency(UserWallet wallet)
        {
            string walletCurrency = wallet.UserWallet_Currency.ToString();
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.LCID);
                if (region.ISOCurrencySymbol == walletCurrency)
                {
                    currencyUsed = region.CurrencySymbol;
                }
            }
        }

        private void RetrieveCurrencyIcon(string currency)
        {
            string newCurrency = null;

            for (int x = 0; x < currency.Length; x++)
            {
                if (x == 0)
                {
                    newCurrency += currency[x];
                }
                else
                {
                    newCurrency += char.ToLower(currency[x]);
                }
            }

            CurrencyIcon.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), "Currency" + newCurrency);
        }

        private void addTransactionBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteTransactionBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NumUpBTN_Click(object sender, RoutedEventArgs e)
        {
            string amount = WalletAmountTB.Text;
            decimal inputAmount = decimal.Parse(amount);
            WalletAmountTB.Text = (inputAmount + 1).ToString();
        }

        private void NumDownBTN_Click(object sender, RoutedEventArgs e)
        {
            string amount = WalletAmountTB.Text;
            decimal inputAmount = decimal.Parse(amount);
            WalletAmountTB.Text = (inputAmount - 1).ToString();
        }

        private void EditWalletBTN_Click(object sender, RoutedEventArgs e)
        {
            var userWallet = (from c in _localdb.db.Customers
                              where c.User_ID == localUID
                              join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                              select u).FirstOrDefault();


        }

    }
}
