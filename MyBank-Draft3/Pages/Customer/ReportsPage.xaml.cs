using MyBank_Draft3.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace MyBank_Draft3.Pages.Customer
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        Database _localdb;
        string localUID;

        public ReportsPage(string UID)
        {
            localUID = UID;
            InitializeComponent();
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            _localdb = new Database();

            RetrieveMonthList();
        }

        private void RetrieveMonthList()
        {
            var monthlyData = from t in _localdb.db.Transactions
                              where t.User_ID == localUID
                              group t by new
                              {
                                  Year = t.Transaction_Date.Year,
                                  Month = t.Transaction_Date.Month,
                              } into g
                              orderby g.Key.Year descending, g.Key.Month descending
                              select new
                              {
                                  Month = g.Key.Month,
                                  Year = g.Key.Year,
                                  Income = ("+" + (g.Where(x => x.Amount > 0).Sum(x => (decimal?)x.Amount) ?? 0).ToString()),
                                  Expense = ("-" + (g.Where(x => x.Amount < 0).Sum(x => (decimal?)Math.Abs(x.Amount)) ?? 0).ToString())
                              };

            var results = monthlyData.ToList().Select(x => new {
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMMM"),
                x.Year,
                x.Income,
                x.Expense
            }).ToList();

            MonthViewData.ItemsSource = results;
        }

        private void MonthViewData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthViewData.SelectedItem != null)
            {
                Design.Visibility = Visibility.Hidden;
                View.Visibility = Visibility.Visible;

                clearBTN.Opacity = 100;
                clearBTN.IsEnabled = true;
                RetrieveTransactions();
            }
        }

        private void RetrieveTransactions()
        {
            var selectedPeriod = (dynamic)MonthViewData.SelectedItem;
            int monthValue = DateTime.ParseExact(selectedPeriod.Month, "MMMM", CultureInfo.InvariantCulture).Month;
            int yearValue = selectedPeriod.Year;

            var monthlyTransactions = (from t in _localdb.db.Transactions
                                       where t.User_ID == localUID
                                       && t.Transaction_Date.Month == monthValue
                                       && t.Transaction_Date.Year == yearValue
                                       join c in _localdb.db.Categories on t.Category_ID equals c.Category_ID
                                       select new
                                       {
                                           Name = t.Transaction_Name,
                                           Date = t.Transaction_Date,
                                           Amount = (c.Category_Type == "Income" ? "+" : "-") + t.Amount,
                                           Type = c.Category_Type
                                       }).ToList();

            TransactionViewData.ItemsSource = monthlyTransactions;
        }

        private void ClearTrigger()
        {
            clearBTN.Opacity = 0;
            clearBTN.IsEnabled = false;
            Design.Visibility = Visibility.Hidden;
            View.Visibility = Visibility.Visible;
        }

        private void clearBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearTrigger();
            MonthViewData.SelectedItem = null;
        }
    }
}
