using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using IBC_Management_System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //IBC_STOREIIEntities db = new IBC_STOREIIEntities();
        IBC_STOREIIEntities db = new IBC_STOREIIEntities();
        public static string usernameChat = string.Empty;
        public static string roleChat = string.Empty;
        private User user = null;
        public Window1()
        {
            InitializeComponent();
            Admin admin = new Admin();
            admin.LoadData();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public static string GetUsername()
        {
            return usernameChat;
        }
        internal string GetRole()
        {
            return roleChat;
        }

        private void Login_Btn_Click(object sender, RoutedEventArgs e)
        {
            string UserName = username.Text;
            string Password = passwords.Password;
            string role = string.Empty;

            var _user = db.Users.FirstOrDefault(u => u.Username == UserName && u.Password == Password);
            if (_user != null)
            {
                usernameChat = _user.Username;
                role = _user.Role.Description;
                roleChat = _user.Role.Description;
                if (_user.Password == "123")
                {
                    Forgotpassword forgotpassword = new Forgotpassword();
                    forgotpassword.SetFirstTimeLogin(true);
                    forgotpassword.SetUsername(_user.Username);
                    forgotpassword.FourthPanel.Visibility = Visibility.Visible;
                    forgotpassword.FirstPanel.Visibility = Visibility.Hidden;
                    forgotpassword.SecondPanel.Visibility = Visibility.Hidden;
                    forgotpassword.ThirdPanel.Visibility = Visibility.Hidden;

                    forgotpassword.Show();
                    this.Close();
                }
                
                else if (_user.Password != "123" && _user.Role.Description == "admin")
                {
                    Admin adminPanel = new Admin(_user.Username);
                    adminPanel.Show();
                    Process.Start("AnnouncementPanel.exe");
                    this.Hide();
                }

                else if (_user.Password != "123" && _user.Role.Description == "reporter")
                {
                    Report reportPanel = new Report(_user.Username);
                    reportPanel.Show();
                    this.Close();
                }

                else if (_user.Password != "123" && _user.Role.Description == "seller")
                {
                    Login_Panel.Visibility = Visibility.Hidden;
                    Branch_ID_Panel.Visibility = Visibility.Visible;
                    var Brancha = db.Branches.Select(a => a.Address);
                    foreach(var temp in Brancha)
                    {
                        Combobox_Branch.Items.Add(temp);
                    }
                    user = _user;
                }
                else if (_user.Password != "123" && _user.Role.Description == "purchasing")
                {
                    Purchase_Form purchase_Form = new Purchase_Form();
                    purchase_Form.UserId = _user.Id;
                    purchase_Form.UserName = _user.Username;

                    purchase_Form.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("INVALID User");
                }
            }
            else
            {
                incorrect_lb.Content = "Incorrect Login!!!";
                incorrect_lb.Foreground = Brushes.Red;
            }

        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            Forgotpassword forgot = new Forgotpassword();
            forgot.Show();
            this.Hide();
        }

        private void passwords_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Combobox_Branch.Text != "") {
                if (db.Branches.Any(c => c.Address == Combobox_Branch.Text))
                {
                    var a = db.Branches.FirstOrDefault(c => c.Address == Combobox_Branch.Text);
                    Sales_Form reportPanel = new Sales_Form();
                    reportPanel.UserId = user.Id;
                    reportPanel.UserName = user.Username;
                    reportPanel.BranchID = a.Id;
                    reportPanel.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("There is no branch ID yet");
                }
            }
            else
            {
                MessageBox.Show("Please select combo box first");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}