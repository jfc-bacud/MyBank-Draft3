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
    /// Interaction logic for StaffList.xaml
    /// </summary>
    public partial class StaffList : Page
    {
        Database _localdb;
        string localUser;
        bool AdminSelected = false;
        string selectedAdminId = "";

        public StaffList(string UID)
        {
            localUser = UID;
            InitializeComponent();
            LoadDatabase();

        }

        private void LoadDatabase()
        {
            _localdb = new Database();
            FetchAllAdmins();
            UpdateAdminCount();
        }

        private void FetchAllAdmins()
        {
            var admins = from a in _localdb.db.Admins
                         where a.User_ID != localUser 
                         select new
                         {
                             AdminID = a.Admin_ID,
                             UserID = a.User_ID,
                             Name = a.Admin_FirstName + " " + a.Admin_LastName,
                             Email = a.Admin_Email,
                             Password = a.Admin_Password,
                             MaskedPassword = MaskPassword(a.Admin_Password)
                         };

            AdminsListView.ItemsSource = admins.ToList();
        }

        private string MaskPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            return new string('*', password.Length);
        }

        private void UpdateAdminCount()
        {
            var adminCount = _localdb.db.Admins.Count();
            totalAdminsTB.Text = adminCount.ToString();
        }

        private void AdminsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AdminsListView.SelectedItem != null)
            {
                AdminSelectionTrigger();

                var selectedAdmin = (dynamic)AdminsListView.SelectedItem;
                firstNameTB.Text = selectedAdmin.Name.Split(' ')[0];
                lastNameTB.Text = selectedAdmin.Name.Split(' ')[1];
                emailTB.Text = selectedAdmin.Email;
                passwordTB.Password = selectedAdmin.Password;
            }
        }

        private void AdminSelectionTrigger()
        {
            AdminSelected = true;
            updateAdminBTN.IsEnabled = true;
            updateAdminBTN.Content = "Update";
            deleteAdminBTN.IsEnabled = true;
            clearSelectionBTN.IsEnabled = true;
            clearSelectionBTN.Opacity = 1;
        }

        private void clearSelectionBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
        }

        private void ClearSelection()
        {
            AdminSelected = false;
            selectedAdminId = "";

            updateAdminBTN.IsEnabled = false;
            deleteAdminBTN.IsEnabled = false;
            clearSelectionBTN.IsEnabled = false;
            clearSelectionBTN.Opacity = 0;

            updateAdminBTN.Content = "Add";
            AdminsListView.SelectedItem = null;
            firstNameTB.Text = "";
            lastNameTB.Text = "";
            emailTB.Text = "";
            passwordTB.Password = "";
        }

        private void updateAdminBTN_Click(object sender, RoutedEventArgs e)
        {
            if (AdminSelected && ValidateAdminInput())
            {
                try
                {
                    var adminToUpdate = _localdb.db.Admins.FirstOrDefault(a => a.Admin_ID == selectedAdminId);

                    if (adminToUpdate != null)
                    {
                        adminToUpdate.Admin_FirstName = firstNameTB.Text.Trim();
                        adminToUpdate.Admin_LastName = lastNameTB.Text.Trim();
                        adminToUpdate.Admin_Email = emailTB.Text.Trim();
                        adminToUpdate.Admin_Password = passwordTB.Password;

                        _localdb.db.SubmitChanges();

                        MessageBox.Show("Admin updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDatabase();
                        ClearSelection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating admin: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    var lastUID = _localdb.db.Users.OrderByDescending(t => t.User_ID).FirstOrDefault();
                    string newUID = "T" + ((int.Parse(lastUID.User_ID.Substring(1))).ToString("D2") + 1);

                    var lastAID = _localdb.db.Admins.OrderByDescending(t => t.Admin_ID).FirstOrDefault();
                    string newAID = "T" + ((int.Parse(lastAID.Admin_ID.Substring(1))).ToString("D2") + 1);

                    var _newUser = new User
                    {
                        User_ID = newUID,
                        Role_ID = "R02"
                    };

                    var _newAdmin = new Classes.Admin
                    {
                        Admin_ID = newAID,
                        User_ID = newUID,
                        Admin_FirstName = firstNameTB.Text,
                        Admin_LastName = lastNameTB.Text,
                        Admin_Email = emailTB.Text,
                        Admin_Password = passwordTB.Password
                    };

                    if (_newUser != null && _newAdmin != null)
                    {
                        _localdb.db.Users.InsertOnSubmit(_newUser);
                        _localdb.db.Admins.InsertOnSubmit(_newAdmin);
                        _localdb.db.SubmitChanges();

                        MessageBox.Show("Admin updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDatabase();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating admin: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void deleteAdminBTN_Click(object sender, RoutedEventArgs e)
        {
            if (AdminSelected)
            {
                var result = MessageBox.Show($"Are you sure you want to delete admin {selectedAdminId}?\n\nThis action cannot be undone.",
                                           "Confirm Delete",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var adminToDelete = _localdb.db.Admins.FirstOrDefault(a => a.Admin_ID == selectedAdminId);

                        if (adminToDelete != null)
                        {
                            _localdb.db.Admins.DeleteOnSubmit(adminToDelete);
                            _localdb.db.SubmitChanges();

                            MessageBox.Show("Admin deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadDatabase();
                            ClearSelection();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting admin: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool ValidateAdminInput()
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

            // Check if email is already taken by another admin
            var existingAdminByEmail = _localdb.db.Admins.FirstOrDefault(a => a.Admin_Email == emailTB.Text.Trim() && a.Admin_ID != selectedAdminId);
            if (existingAdminByEmail != null)
            {
                MessageBox.Show("This email address is already registered to another admin.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
