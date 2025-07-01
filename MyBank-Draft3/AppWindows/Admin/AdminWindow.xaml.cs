using MyBank_Draft3.Classes;
using MyBank_Draft3.Pages.Admin;
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

namespace MyBank_Draft3.AppWindows.Admin
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        CustomerList customerPage;
        StaffList staffPage;
        SettingsStaff settingsPage;
        Database _localdb;
        string localUser;

        public AdminWindow(string userEmail)
        {
            InitializeComponent();
            RetrieveUser(userEmail);
            FirstPage();
        }

        private void RetrieveUser(string userEmail)
        {
            _localdb = new Database();

            var user = (from c in _localdb.db.Admins
                        where c.Admin_Email == userEmail
                        select c.User_ID).FirstOrDefault();

            localUser = user.ToString();
        }

        private void FirstPage()
        {
            customerPage = new CustomerList();
            adminWindowFrame.Content = customerPage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearBTNColor(sender as Button);
            customerPage = new CustomerList();
            adminWindowFrame.Content = customerPage;
            SelectedBTNColor(sender as Button);
        }

        private void ClearBTNColor(Button button)
        {
            string hexCode = "#161d24";
            Color color = (Color)ColorConverter.ConvertFromString(hexCode);

            customerListBTN.Background = new SolidColorBrush(color);
            staffBTN.Background = new SolidColorBrush(color);
            SettingsBTN.Background = new SolidColorBrush(color);
        }

        private void SelectedBTNColor(Button button)
        {
            button.Background = new SolidColorBrush(Colors.Green);
        }

        private void staffBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearBTNColor(sender as Button);
            staffPage = new StaffList(localUser);
            adminWindowFrame.Content = staffPage;
            SelectedBTNColor(sender as Button);
        }

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearBTNColor(sender as Button);
            settingsPage = new SettingsStaff(localUser);
            adminWindowFrame.Content = settingsPage;
            SelectedBTNColor(sender as Button);
        }
    }
}
