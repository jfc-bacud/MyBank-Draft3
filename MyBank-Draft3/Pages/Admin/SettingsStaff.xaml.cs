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

namespace MyBank_Draft3.Pages.Admin
{
    /// <summary>
    /// Interaction logic for SettingsStaff.xaml
    /// </summary>
    public partial class SettingsStaff : Page
    {
        Database _localdb;
        string localUID;

        public SettingsStaff(string UID)
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

        private void ViewAccount()
        {
            var admins = (from a in _localdb.db.Admins
                          where a.User_ID == localUID
                          select a).FirstOrDefault();

            userTB.Text = admins.Admin_Email;
            FirstTB.Text = admins.Admin_FirstName;
            LastTB.Text = admins.Admin_LastName;
            EmailTB.Text = admins.Admin_Email;
            PasswordTB.Text = admins.Admin_Password;

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
            var values = (from a in _localdb.db.Admins
                          where a.User_ID == localUID
                          select new
                          {
                              a.Admin_ID,
                              a.User_ID
                          }).FirstOrDefault();


            var staffToDelete = _localdb.db.Admins.SingleOrDefault(t => t.Admin_ID == values.Admin_ID);
            var userToDelete = _localdb.db.Users.SingleOrDefault(t => t.User_ID == values.User_ID);
            

            _localdb.db.Admins.DeleteOnSubmit(staffToDelete);
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
