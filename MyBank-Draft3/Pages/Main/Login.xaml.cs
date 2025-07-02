using MyBank_Draft3.AppWindows.Main;
using MyBank_Draft3.AppWindows.Customer;
using MyBank_Draft3.AppWindows.Admin
    ;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        Database db;
        string role;

        public Login()
        {
            InitializeComponent();
            LoadDatabase();
        }
        public void LoadDatabase()
        {
            db = new Database();
        }
        private void loginBTN_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(userIN.Text))
            {
                ThrowError(1);
                return;
            }

            if (String.IsNullOrEmpty(passIN.Password))
            {
                ThrowError(2);
                return;
            }

            if (UserExists(out role))
            {
                if (VerifiedPassword(role))
                {
                    OpenWindow(role);
                }
                else
                {
                    ThrowError(4);
                    return;
                }
            }
            else
            {
                ThrowError(3);
                return;
            }
        }

        private void ThrowError(int error)
        {
            switch (error)
            {
                case 1:
                    ErrorUser.Content = "*User field is required";
                    ErrorUser.Visibility = Visibility.Visible;
                    break;

                case 2:
                    ErrorUser.Content = "*Password field is required";
                    ErrorUser.Visibility = Visibility.Visible;
                    break;

                case 3:
                    ErrorUser.Content = "*Invalid username";
                    ErrorUser.Visibility = Visibility.Visible;
                    break;

                case 4:
                    ErrorUser.Content = "*Incorrect password";
                    ErrorUser.Visibility = Visibility.Visible;
                    break;
            }
        }

        private bool UserExists(out string role)
        {
            var possibleCustomer = (from c in db.GetCustomer()
                                    where c.Customer_Email == userIN.Text
                                    select c).FirstOrDefault();

            var possibleAdmin = (from a in db.GetAdmin()
                                 where a.Admin_Email == userIN.Text
                                 select a).FirstOrDefault();

            if (possibleCustomer != null)
            {
                role = "Customer";
                return true;
            }
            else if (possibleAdmin != null)
            {
                role = "Admin";
                return true;
            }
            else
            {
                role = null;
                return false;
            }
        }
        private bool VerifiedPassword(string role)
        {
            if (role == "Customer")
            {
                var user = (from c in db.GetCustomer()
                            where c.Customer_Email == userIN.Text
                            select c).FirstOrDefault();

                if (user.Customer_Password == passIN.Password)
                {
                    return true;
                }
            }
            else if (role == "Admin")
            {
                var user = (from a in db.GetAdmin()
                            where a.Admin_Email == userIN.Text
                            select a).FirstOrDefault();

                if (user.Admin_Password == passIN.Password)
                {
                    return true;
                }
            }

            return false;
        }
        private void OpenWindow(string role)
        {
            if (role == "Customer")
            {
                CustomerWindow customerWindow = new CustomerWindow(userIN.Text);
                customerWindow.Show();
                Window.GetWindow(this).Close();
            }
            else if (role == "Admin")
            {
                AdminWindow adminWindow = new AdminWindow(userIN.Text);
                adminWindow.Show();
                Window.GetWindow(this).Close();
            }
        }
        private void newBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main)
            {
                main.NavToNew();
            }
        }

        private void OnHover(object sender, MouseEventArgs e)
        {
            var ButtonOn = sender as Button;

            ButtonOn.Background = new SolidColorBrush(Colors.Green);
            ButtonOn.Foreground = new SolidColorBrush(Colors.White);
        }

        private void OnLeave(object sender, MouseEventArgs e)
        {
            var ButtonOn = sender as Button;

            ButtonOn.Background = new SolidColorBrush(Colors.DarkGreen);
            ButtonOn.Foreground = new SolidColorBrush(Colors.White);
        }

        private void userIN_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorUser.Visibility = Visibility.Hidden;
            PassError.Visibility = Visibility.Hidden;
        }

        private void passIN_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorUser.Visibility = Visibility.Hidden;
            PassError.Visibility = Visibility.Hidden;
        }
    }
}
