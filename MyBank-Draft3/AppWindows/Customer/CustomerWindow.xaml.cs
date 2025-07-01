using MyBank_Draft3.Classes;
using MyBank_Draft3.Pages.Customer;
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
using System.Windows.Shapes;

namespace MyBank_Draft3.AppWindows.Customer
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        Home homePage;
        Database _localdb;
        TransactionsPage transactionPage;
        ReportsPage reportsPage;
        string localUser;

        public CustomerWindow(string userEmail)
        {
            InitializeComponent();
            RetrieveUser(userEmail);
            ViewHome();
        }

        public void ViewHome()
        {
            homeBTN.Background = new SolidColorBrush(Colors.Green);
            homePage = new Home(localUser);
            windowFrame.Content = homePage;
        }

        public void RetrieveUser(string userEmail)
        {
            _localdb = new Database();

            var user = (from c in _localdb.db.Customers
                        where c.Customer_Email == userEmail
                        select c.User_ID).FirstOrDefault();

            localUser = user.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearBTNColor(sender as Button);
            homePage = new Home(localUser);
            windowFrame.Content = homePage;
            SelectedBTNColor(sender as Button);
        }

        private void TransactionsBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearBTNColor(sender as Button);
            transactionPage = new TransactionsPage(localUser);
            windowFrame.Content = transactionPage;
            SelectedBTNColor(sender as Button);
        }

        private void ClearBTNColor(Button button)
        {
            string hexCode = "#161d24";
            Color color = (Color)ColorConverter.ConvertFromString(hexCode);

            homeBTN.Background = new SolidColorBrush(color);
            TransactionsBTN.Background = new SolidColorBrush(color);
            Reports.Background = new SolidColorBrush(color);
        }

        private void SelectedBTNColor(Button button)
        {
            button.Background = new SolidColorBrush(Colors.Green);
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            ClearBTNColor(sender as Button);
            reportsPage = new ReportsPage(localUser);
            windowFrame.Content = reportsPage;
            SelectedBTNColor(sender as Button);
        }
    }
}
