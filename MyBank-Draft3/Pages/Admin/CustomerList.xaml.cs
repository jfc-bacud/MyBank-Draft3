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

namespace MyBank_Draft3.Pages.Admin
{
    /// <summary>
    /// Interaction logic for CustomerList.xaml
    /// </summary>
    public partial class CustomerList : Page
    {
        Database _localdb;
        bool UserSelected = false;
        string selectedUserId;


        public CustomerList()
        {
            InitializeComponent();
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            _localdb = new Database();
            FetchAllUsers();
            UpdateUserCount();
        }

        private void FetchAllUsers()
        {
            var users = from c in _localdb.db.Customers
                        select new
                        {
                            UserID = c.User_ID,
                            Name = c.Customer_FirstName + " " + c.Customer_LastName,
                            Email = c.Customer_Email,
                            Password = c.Customer_Password,
                            MaskedPassword = MaskPassword(c.Customer_Password)
                        };

            UsersListView.ItemsSource = users.ToList();
        }

        private string MaskPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            return new string('*', password.Length);
        }

        private void UpdateUserCount()
        {
            var userCount = _localdb.db.Customers.Count();
            totalUsersTB.Text = userCount.ToString();
        }

        private void UsersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListView.SelectedItem != null)
            {
                UserSelectionTrigger();

                var selectedUser = (dynamic)UsersListView.SelectedItem;
                selectedUserId = selectedUser.UserID;

                userIdTB.Text = selectedUser.UserID;
                firstNameTB.Text = selectedUser.Name.Split(' ')[0];
                lastNameTB.Text = selectedUser.Name.Split(' ')[1];
                emailTB.Text = selectedUser.Email;
                passwordTB.Password = selectedUser.Password;
            }
        }

        private void UserSelectionTrigger()
        {
            UserSelected = true;
            updateUserBTN.IsEnabled = true;
            deleteUserBTN.IsEnabled = true;
            clearSelectionBTN.IsEnabled = true;
            clearSelectionBTN.Opacity = 1;
        }

        private void clearSelectionBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
        }

        private void ClearSelection()
        {
            UserSelected = false;
            selectedUserId = "";

            updateUserBTN.IsEnabled = false;
            deleteUserBTN.IsEnabled = false;
            clearSelectionBTN.IsEnabled = false;
            clearSelectionBTN.Opacity = 0;

            UsersListView.SelectedItem = null;
            userIdTB.Text = "";
            firstNameTB.Text = "";
            lastNameTB.Text = "";
            emailTB.Text = "";
            passwordTB.Password = "";
        }

        private void updateUserBTN_Click(object sender, RoutedEventArgs e)
        {
            if (UserSelected && ValidateUserInput())
            {
                try
                {
                    var userToUpdate = _localdb.db.Customers.FirstOrDefault(c => c.User_ID == selectedUserId);

                    if (userToUpdate != null)
                    {
                        userToUpdate.Customer_FirstName = firstNameTB.Text.Trim();
                        userToUpdate.Customer_LastName = lastNameTB.Text.Trim();
                        userToUpdate.Customer_Email = emailTB.Text.Trim();
                        userToUpdate.Customer_Password = passwordTB.Password;

                        _localdb.db.SubmitChanges();

                        MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDatabase();
                        ClearSelection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void deleteUserBTN_Click(object sender, RoutedEventArgs e)
        {
            if (UserSelected)
            {
                var result = MessageBox.Show($"Are you sure you want to delete user {selectedUserId}?\n\nThis action cannot be undone and will also delete all associated transactions and wallet data.",
                                           "Confirm Delete",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Delete user's transactions first
                        var userTransactions = _localdb.db.Transactions.Where(t => t.User_ID == selectedUserId);
                        _localdb.db.Transactions.DeleteAllOnSubmit(userTransactions);

                        // Delete user's wallet
                        var userToDelete = _localdb.db.Customers.FirstOrDefault(c => c.User_ID == selectedUserId);
                        if (userToDelete != null && userToDelete.UserWallet_ID != null)
                        {
                            var userWallet = _localdb.db.UserWallets.FirstOrDefault(w => w.UserWallet_ID == userToDelete.UserWallet_ID);
                            if (userWallet != null)
                            {
                                _localdb.db.UserWallets.DeleteOnSubmit(userWallet);
                            }
                        }

                        // Finally delete the user
                        if (userToDelete != null)
                        {
                            _localdb.db.Customers.DeleteOnSubmit(userToDelete);
                        }

                        _localdb.db.SubmitChanges();

                        MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDatabase();
                        ClearSelection();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool ValidateUserInput()
        {
            if (string.IsNullOrWhiteSpace(firstNameTB.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                firstNameTB.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(lastNameTB.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                lastNameTB.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailTB.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                emailTB.Focus();
                return false;
            }

            if (!IsValidEmail(emailTB.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                emailTB.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(passwordTB.Password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                passwordTB.Focus();
                return false;
            }

            // Check if email is already taken by another user
            var existingUser = _localdb.db.Customers.FirstOrDefault(c => c.Customer_Email == emailTB.Text.Trim() && c.User_ID != selectedUserId);
            if (existingUser != null)
            {
                MessageBox.Show("This email address is already registered to another user.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                emailTB.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
