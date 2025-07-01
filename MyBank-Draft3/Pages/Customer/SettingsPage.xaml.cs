using MaterialDesignThemes.Wpf;
using MyBank_Draft3.AppWindows.Main;
using MyBank_Draft3.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
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
            localUID = UID;
            InitializeComponent();
            LoadDatabase();
            ViewAccount();
        }

        private void LoadDatabase()
        {
            _localdb = new Database();
        }

        private void AccountBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearButtonColor();
            ViewAccount();
            ColorOnClick(sender as Button);
        }

        private void ViewAccount()
        {
            AccountView.Visibility = Visibility.Visible;
            GeneralView.Visibility = Visibility.Hidden;

            var customer = (from c in _localdb.db.Customers
                            where c.User_ID == localUID
                            select c).FirstOrDefault();

            userTB.Text = customer.Customer_Email;
            FirstTB.Text = customer.Customer_FirstName;
            LastTB.Text = customer.Customer_LastName;
            EmailTB.Text = customer.Customer_Email;
            PasswordTB.Text = customer.Customer_Password;
            
        }

        private void GeneralBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearButtonColor();
            AccountView.Visibility = Visibility.Hidden;
            GeneralView.Visibility = Visibility.Visible;

            var userWallet = (from c in _localdb.db.Customers
                              where c.User_ID == localUID
                              join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                              select u).FirstOrDefault();

            PopulateCB();
            RetrieveWallet();

            WalletAmountTB.Text = userWallet.UserWallet_Balance.ToString();
            CurrencyCB.SelectedItem = userWallet.UserWallet_Currency;
            ColorOnClick(sender as Button);
        }

        private void PopulateCB()
        {
            List<string> list = new List<string> { "USD", "EUR", "PHP", "JPY", "KRW", "GBP", "CNY" };
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
            if (WalletAmountTB.Text != null && CurrencyCB.SelectedItem != null)
            {
                var userWallet = (from c in _localdb.db.Customers
                                  where c.User_ID == localUID
                                  join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                                  select u).FirstOrDefault();

                userWallet.UserWallet_Balance = decimal.Parse(WalletAmountTB.Text);
                userWallet.UserWallet_Currency = CurrencyCB.SelectedItem.ToString();

                SubmitWalletChanges();
            }
        }

        private void SubmitWalletChanges()
        {
            _localdb.db.SubmitChanges();
            LoadDatabase();
            RetrieveWallet();
        }

        private void ColorOnClick(Button button)
        {
            button.Background = new SolidColorBrush(Colors.White);

            if (button.Name.ToString() == "GeneralBTN")
            {
                GearIcon.Foreground = new SolidColorBrush(Colors.Black);
                GeneralTB.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                AccountIcon.Foreground = new SolidColorBrush(Colors.Black);
                AccountTB.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void ClearButtonColor()
        {
            string hexCode = "#12171d";
            Color color = (Color)ColorConverter.ConvertFromString(hexCode);

            GearIcon.Foreground = new SolidColorBrush(Colors.White);
            GeneralTB.Foreground = new SolidColorBrush(Colors.White);
            GeneralBTN.Background = new SolidColorBrush(color);

            AccountBTN.Background = new SolidColorBrush(color);
            AccountIcon.Foreground = new SolidColorBrush(Colors.White);
            AccountTB.Foreground = new SolidColorBrush(Colors.White); 
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            var customerToEdit = (from c in _localdb.db.Customers
                                 where c.Customer_ID == localUID
                                 select c).FirstOrDefault();

            customerToEdit.Customer_FirstName = FirstTB.Text;
            customerToEdit.Customer_LastName = LastTB.Text;
            customerToEdit.Customer_Email = EmailTB.Text;
            customerToEdit.Customer_Password = PasswordTB.Text;

            SubmitChanges();
        }

        private void SubmitChanges()
        {
            _localdb.db.SubmitChanges();
            LoadDatabase();
            ViewAccount();
        }

        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            var values = (from c in _localdb.db.Customers
                              where c.User_ID == localUID
                              join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                              select new
                              {
                                  c.Customer_ID,
                                  c.User_ID,
                                  u.UserWallet_ID
                              }).FirstOrDefault();

            var transactionsToDelete = _localdb.db.Transactions
           .Where(t => t.User_ID == values.User_ID)
           .ToList();

            var customerToDelete = _localdb.db.Customers.SingleOrDefault(t => t.Customer_ID == values.Customer_ID);
            var userToDelete = _localdb.db.Users.SingleOrDefault(t => t.User_ID == values.User_ID);
            var walletToDelete = _localdb.db.UserWallets.SingleOrDefault(t => t.UserWallet_ID == values.UserWallet_ID);

            _localdb.db.Transactions.DeleteAllOnSubmit(transactionsToDelete);
            _localdb.db.Customers.DeleteOnSubmit(customerToDelete);
            _localdb.db.UserWallets.DeleteOnSubmit(walletToDelete);
            _localdb.db.Users.DeleteOnSubmit(userToDelete);
            DeleteAccount();
        }

        private void DeleteAccount()
        {
            _localdb.db.SubmitChanges();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Window.GetWindow(this).Close();
        }
    }
}
