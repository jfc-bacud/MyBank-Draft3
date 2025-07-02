using Microsoft.Xaml.Behaviors.Core;
using MyBank_Draft3.AppWindows.Customer;
using MyBank_Draft3.AppWindows.Main;
using MyBank_Draft3.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MyBank_Draft3.Pages.Main
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Page
    {
        Database _localdb;

        public SignUp()
        {
            InitializeComponent();
            LoadDatabase();
        }

        void LoadDatabase()
        {
            _localdb = new Database();
            PopulateCurrencyCB();
        }

        void PopulateCurrencyCB()
        {
            List<string> strings = new List<string> { "USD", "EUR", "PHP", "JPY", "KRW", "GBP", "CNY" };
            CurrencyCB.ItemsSource = strings; 
        }


        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fnameIN.Text))
            {
                MessageBox.Show("First name field cannot be left blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                fnameIN.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(lnameIN.Text))
            {
                MessageBox.Show("Last name field cannot be left blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                lnameIN.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(emailIN.Text))
            {
                MessageBox.Show("Email field cannot be left blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                emailIN.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(passIN.Text))
            {
                MessageBox.Show("Password field cannot be left blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                passIN.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrencyCB.SelectedItem.ToString()))
            {
                MessageBox.Show("Currency field cannot be left blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CurrencyCB.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(Amount.Value.ToString()))
            {
                MessageBox.Show("Amount field cannot be left blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CurrencyCB.Focus();
                return;
            }

            AddUser();
        }

        private void AddUser()
        {
            if (!_localdb.db.Customers.Any(c => c.Customer_Email == emailIN.Text.ToString()))
            {
                var lastUID = _localdb.db.Users.OrderByDescending(u => u.User_ID).FirstOrDefault();
                var lastCID = _localdb.db.Customers.OrderByDescending(c => c.Customer_ID).FirstOrDefault();

                string newCID = "C" + (int.Parse(lastCID.Customer_ID.Substring(1)) + 1).ToString("D2") ;
                string newUID = "U" + (int.Parse(lastUID.User_ID.Substring(1)) + 1).ToString("D2");
                int UWID = GenerateNum();


                var _newUser = new User
                {
                    User_ID = newUID,
                    Role_ID = "R01"
                };

                var _newUserWallet = new UserWallet
                {
                    UserWallet_ID = UWID,
                    UserWallet_Balance = decimal.Parse(Amount.Value.ToString()),
                    UserWallet_Currency = CurrencyCB.SelectedItem.ToString()
                };

                var _newCustomer = new Classes.Customer
                {
                    Customer_ID = newCID,
                    User_ID = newUID,
                    UserWallet_ID = UWID,
                    Customer_FirstName = fnameIN.Text,
                    Customer_LastName = lnameIN.Text,
                    Customer_Email = emailIN.Text,
                    Customer_Password = passIN.Text,
                };

                _localdb.db.Users.InsertOnSubmit(_newUser);
                _localdb.db.UserWallets.InsertOnSubmit(_newUserWallet);
                _localdb.db.Customers.InsertOnSubmit(_newCustomer);
                SubmitChanges();
            }
        }

        private int GenerateNum()
        {
            Random rnd = new Random();
            int generatedNum;

            while (true)
            {
                generatedNum = rnd.Next(0, 9999999);

                if (!_localdb.db.UserWallets.Any(u => u.UserWallet_ID == generatedNum))
                {
                    break;
                }
            }

            return generatedNum;
        }

        private void SubmitChanges()
        {
            _localdb.db.SubmitChanges();
            LoadDatabase();
            MessageBox.Show("Added user successfully! Welcome to MyBank!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            Launch();
        }

        private void Launch()
        {
            CustomerWindow customerWindow = new CustomerWindow(emailIN.Text);
            customerWindow.Show();
            Window.GetWindow(this).Close();
        }

        private void returnBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavBack();
            }
        }
    }
}
