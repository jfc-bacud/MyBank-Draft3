﻿using MaterialDesignThemes.Wpf;
using MyBank_Draft3.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for TransactionsPage.xaml
    /// </summary>
    public partial class TransactionsPage : Page
    {
        string localUID;
        Database _localdb;
        string currencyUsed;
        bool SomethingSelected = false;

        public TransactionsPage(string UID)
        {
            localUID = UID;
            InitializeComponent();
            LoadDatabase();
        }

        private void LoadDatabase()
        {          
            _localdb = new Database();
            FetchAvailableCategories();
            FetchTransactions();
            RetrieveWallet();
        }

        private void FetchAvailableCategories()
        {
            var _categories = (from c in _localdb.db.Categories
                               select c.Category_Type).ToList();


            categoryCB.ItemsSource = _categories;
        }

        private void FetchTransactions()
        {
            var _transactions = from t in _localdb.db.Transactions
                                where t.User_ID == localUID
                                join c in _localdb.db.Categories on t.Category_ID equals c.Category_ID
                                select new 
                                {
                                    ID = t.Transaction_ID,
                                    Transaction = t.Transaction_Name,
                                    Date = t.Transaction_Date,
                                    Amount = (c.Category_Type == "Income" ? "+" : "-") + t.Amount,
                                    Type = c.Category_Type,
                                    Desc = t.Transaction_Desc
                                };

            ListViewData.ItemsSource = _transactions.ToList();
        }

        private void ListViewData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewData.SelectedItem != null)
            {
                SelectedTrigger();

                var selectedTransaction = (dynamic)ListViewData.SelectedItem;

                NameTB.Text = selectedTransaction.Transaction;
                categoryCB.SelectedItem = selectedTransaction.Type;
                datePicker.SelectedDate = selectedTransaction.Date;
                string amount = selectedTransaction.Amount.Remove(0,1);
                AmountTB.Text = amount;
                DescriptionTB.Text = selectedTransaction.Desc;
            }
        }

        private void NumUpBTN_Click(object sender, RoutedEventArgs e)
        {
            string amount = AmountTB.Text;
            decimal inputAmount = decimal.Parse(amount);
            AmountTB.Text = (inputAmount + 1).ToString();
        }

        private void NumDownBTN_Click(object sender, RoutedEventArgs e)
        {
            string amount = AmountTB.Text;
            decimal inputAmount = decimal.Parse(amount);

            if (inputAmount > 0)
            {
                AmountTB.Text = (inputAmount - 1).ToString();
            }
            else
            {
                return;
            }
        }

        private void SelectedTrigger()
        {
            SomethingSelected = true;
            deleteTransactionBTN.IsEnabled = true;
            addButtonTB.Text = "Edit";
            clearBTN.IsEnabled = true;
            clearBTN.Opacity = 100;
        }

        private void clearBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearTrigger();
        }

        private void ClearTrigger()
        {
            clearBTN.IsEnabled = false;
            deleteTransactionBTN.IsEnabled = false;
            clearBTN.Opacity = 0;

            SomethingSelected = false;
            addButtonTB.Text = "Add";
            categoryCB.SelectedItem = null;
            ListViewData.SelectedItem = null;
            datePicker.SelectedDate = null;
            AmountTB.Text = "0";
            DescriptionTB.Text = null;
        }

        private void addTransactionBTN_Click(object sender, RoutedEventArgs e)
        {
            if (SomethingSelected) // For Editing
            {
                if (FieldVerify())
                {
                    var selectedTransaction = (dynamic)ListViewData.SelectedItem;
                    string transactionID = selectedTransaction.ID;
                    decimal oldAmount = decimal.Parse(selectedTransaction.Amount.Remove(0, 1));
                    string categoryDesc = categoryCB.SelectedItem.ToString();

                    var _categoryDefined = (from c in _localdb.db.Categories
                                            where c.Category_Type == categoryDesc
                                            select c.Category_ID).FirstOrDefault();


                    var _editedtransaction = (from t in _localdb.db.Transactions
                                              where t.Transaction_ID == transactionID
                                              select t).FirstOrDefault();

                    _editedtransaction.Transaction_Name = NameTB.Text;
                    _editedtransaction.Category_ID = _categoryDefined;
                    _editedtransaction.Amount = decimal.Parse(AmountTB.Text.ToString());
                    _editedtransaction.Transaction_Date = datePicker.SelectedDate.Value;
                    _editedtransaction.Transaction_Desc = DescriptionTB.Text.ToString();

                    ChangeWalletDifference(oldAmount, out bool validTransaction);

                    if (validTransaction)
                    {
                        MessageBox.Show("Transaction edited successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        SubmitChanges();
                        ClearTrigger();
                    }
                    else
                    {
                        ThrowError(1);
                    }
                }
                
            }
            else // For adding
            {
                if (FieldVerify())
                {
                    var lastTID = _localdb.db.Transactions.OrderByDescending(t => t.Transaction_ID).FirstOrDefault();
                    string newTID = "T" + ((int.Parse(lastTID.Transaction_ID.Substring(1))).ToString("D2") + 1);

                    string categoryDesc = categoryCB.SelectedItem.ToString();
                    var _categoryDefined = (from c in _localdb.db.Categories
                                            where c.Category_Type == categoryDesc
                                            select c.Category_ID).FirstOrDefault();


                    var userwalltet = (from c in _localdb.db.Customers
                                       where c.User_ID == localUID
                                       select c.UserWallet_ID).FirstOrDefault();

                    var _newTransaction = new Transaction
                    {
                        Transaction_ID = newTID,
                        User_ID = localUID,
                        Transaction_Name = NameTB.Text,
                        Category_ID = _categoryDefined,
                        Amount = decimal.Parse(AmountTB.Text),
                        Transaction_Desc = DescriptionTB.Text,
                        Transaction_Date = datePicker.SelectedDate.Value,
                    };

                    ChangeWallet(out bool validTransaction);

                    if (validTransaction)
                    {
                        MessageBox.Show("Transaction added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        _localdb.db.Transactions.InsertOnSubmit(_newTransaction);
                        SubmitChanges();
                    }
                    else
                    {
                        ThrowError(1);
                    }
                }
            }
        }

        private void ThrowError(int error)
        {
            switch(error)
            {
                case 1:
                    MessageBox.Show("Transaction cannot be done due to insufficient funds!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                case 2:
                    MessageBox.Show("Name field cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameTB.Focus();
                    break;

                case 3:
                    MessageBox.Show("Category field cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    categoryCB.Focus();
                    break;

                case 4:
                    MessageBox.Show("Date field cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    datePicker.Focus();
                    break;

                case 5:
                    MessageBox.Show("Date is in the wrong format!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    datePicker.Focus();
                    break;

                case 6:
                    MessageBox.Show("Amount field cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    AmountTB.Focus();
                    break;

                case 7:
                    MessageBox.Show("Amount needs to be a number!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    AmountTB.Focus();
                    break;

                case 8:
                    MessageBox.Show("Transaction cannot be done due to insufficient funds!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }

        private void SubmitChanges()
        {
            try
            {
                _localdb.db.SubmitChanges();
                LoadDatabase();
            }
            catch
            {
                return;
            }
        }

        private bool FieldVerify() // FOR ERROR HANDLING LATER
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
            {
                ThrowError(2);
                return false;

            }

            if (categoryCB.SelectedItem == null)
            {
                ThrowError(3);
                return false;
            }

            if (datePicker.SelectedDate.Value == null)
            {
                ThrowError(4);
                return false;
            }
            else
            {
                if (!DateTime.TryParseExact(datePicker.SelectedDate.Value.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime result))
                {
                    ThrowError(5);
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(AmountTB.Text))
            {
                ThrowError(6);
            }
            else
            {
                if (!decimal.TryParse(AmountTB.Text, out decimal result))
                {
                    ThrowError(7);
                    return false;
                }
                else
                {
                    if (result < 0)
                    {
                        ThrowError(8);
                        return false;
                    }
                }
            }

            return true;
        }


        private void deleteTransactionBTN_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to delete this transaction?\n\nThis action cannot be undone.",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {

                var selectedTransaction = (dynamic)ListViewData.SelectedItem;
                string _localTransaction = selectedTransaction.ID;

                var transactionToDelete = _localdb.db.Transactions.SingleOrDefault(t => t.Transaction_ID == _localTransaction);
                _localdb.db.Transactions.DeleteOnSubmit(transactionToDelete);

                MessageBox.Show("Transaction deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ChangeWalletDelete();
                SubmitChanges();
                ClearTrigger();
            }
        }

        private void ChangeWallet(out bool valid)
        {
            string _categoryType = categoryCB.SelectedItem.ToString();

            var _userWallet = (from u in _localdb.db.Customers
                              where u.User_ID == localUID
                              join w in _localdb.db.UserWallets on u.UserWallet_ID equals w.UserWallet_ID
                              select w).FirstOrDefault();

            if (_categoryType.ToString() == "Income")
            {
                _userWallet.UserWallet_Balance += decimal.Parse(AmountTB.Text);
            }
            else if (_categoryType.ToString() == "Expense")
            {
                if (_userWallet.UserWallet_Balance - decimal.Parse(AmountTB.Text) > 0)
                {
                    _userWallet.UserWallet_Balance -= decimal.Parse(AmountTB.Text);
                }
                else
                {
                    valid = false;
                    return;
                }

            }

            valid = true;
        }

        private void ChangeWalletDifference(decimal oldTransaction, out bool valid)
        {
            var _categoryType = (dynamic)ListViewData.SelectedItem;
            string type = _categoryType.Type.ToString();

            var _userWallet = (from u in _localdb.db.Customers
                               where u.User_ID == localUID
                               join w in _localdb.db.UserWallets on u.UserWallet_ID equals w.UserWallet_ID
                               select w).FirstOrDefault();

            decimal newAmount = decimal.Parse(AmountTB.Text);
            decimal oldAmount = oldTransaction;

            if (type == "Income")
            {
                if (oldAmount < newAmount)
                {
                    _userWallet.UserWallet_Balance += (newAmount - oldAmount);
                }
                else
                {
                    if (_userWallet.UserWallet_Balance - (newAmount - oldAmount) > 0)
                    {
                        _userWallet.UserWallet_Balance -= (oldAmount - newAmount);
                    }
                    else
                    {
                        valid = false;
                        return;
                    }

                }
            }
            else
            {
                if (oldAmount < newAmount)
                {
                    if (_userWallet.UserWallet_Balance - (newAmount - oldAmount) > 0)
                    {
                        _userWallet.UserWallet_Balance -= (newAmount - oldAmount);
                    }
                    else
                    {
                        valid = false;
                        return;
                    }
                }
                else
                {
                    _userWallet.UserWallet_Balance += (oldAmount - newAmount);
                }
            }

            valid = true;
        }

        private void ChangeWalletDelete()
        {
            var _userWallet = (from u in _localdb.db.Customers
                               where u.User_ID == localUID
                               join w in _localdb.db.UserWallets on u.UserWallet_ID equals w.UserWallet_ID
                               select w).FirstOrDefault();

            _userWallet.UserWallet_Balance += decimal.Parse(AmountTB.Text);
            SubmitChanges();
        }

        private void RetrieveWallet()
        {
            var userWallet = (from c in _localdb.db.Customers
                              where c.User_ID == localUID
                              join u in _localdb.db.UserWallets on c.UserWallet_ID equals u.UserWallet_ID
                              select u).FirstOrDefault();

            RetrieveCurrency(userWallet);

            balanceTB.Text = currencyUsed + userWallet.UserWallet_Balance.ToString();

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
    }
}
