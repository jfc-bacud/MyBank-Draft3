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

namespace MyBank_Draft3.Pages.Customer
{
    /// <summary>
    /// Interaction logic for TransactionsPage.xaml
    /// </summary>
    public partial class TransactionsPage : Page
    {
        string localUID;
        Database _localdb;

        public TransactionsPage(string UID)
        {
            localUID = UID;
            InitializeComponent();
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            _localdb = new Database();
            FetchTransactions();

        }

        private void FetchTransactions()
        {
            var _transactions = from t in _localdb.db.Transactions
                                where t.User_ID == localUID
                                join c in _localdb.db.Categories on t.Category_ID equals c.Category_ID
                                select new 
                                {
                                    c.Category_Desc,
                                    t.Transaction_Date,
                                    t.Amount,
                                    c.Category_Type,
                                    t.Transaction_Desc,
                                };

            ListViewData.ItemsSource = _transactions.ToList();


        }
    }
}
