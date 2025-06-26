using MyBank_Draft3.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        public ObservableCollection<TransactionListView> _recordedTransactions { get; set; }

        public TransactionsPage(string UID)
        {
            _recordedTransactions = new ObservableCollection<TransactionListView>();
            localUID = UID;
            InitializeComponent();
            LoadDatabase();
        }

        private void LoadDatabase()
        {          
            _localdb = new Database();
            this.DataContext = this;
            FetchTransactions();
          
        }

        private void FetchTransactions()
        {
            var _transactions = (from t in _localdb.db.Transactions
                                where t.User_ID == localUID
                                join c in _localdb.db.Categories on t.Category_ID equals c.Category_ID
                                select new TransactionListView
                                {
                                    Transact = c.Category_Desc,
                                    Date = t.Transaction_Date,
                                    Amount = t.Amount,
                                    Type = c.Category_Type,
                                    //TransactionDesc = t.Transaction_Desc
                                }).ToList();

            _recordedTransactions.Clear();

            foreach (var t in _transactions)
            {
                _recordedTransactions.Add(t);
            }

        }

        //private void CreateItemSource(IQueryable<TransactionListView> transactions)
        //{
        //    DataTable itemSource = new DataTable();

        //    itemSource.Columns.Add("Transaction", typeof(string));
        //    itemSource.Columns.Add("Date", typeof(DateTime));
        //    itemSource.Columns.Add("Amount", typeof(decimal));
        //    itemSource.Columns.Add("Type", typeof(string));

        //    foreach (var t in transactions)
        //    {
        //        itemSource.Rows.Add(t.CategoryDesc, t.TransactionDate, t.Amount, t.CategoryType);
        //    }

        //    ListViewData.ItemsSource = itemSource.DefaultView;

        //}
    }
}
