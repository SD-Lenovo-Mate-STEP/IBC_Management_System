using System;
using System.Deployment.Internal;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Forgotpassword.xaml
    /// </summary>
    public partial class Forgotpassword : Window
    {
        private string secretRandomNumber = string.Empty;
        bool isEmailFound = false;
        bool isPhoneNumberFound = false;
        public static string username;
        public static bool IsFirstTimeLogin = false;
        public Forgotpassword()
        {
            InitializeComponent();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Window1 main = new Window1();
            main.Show();
            Close();
        }
        internal void SetFirstTimeLogin(bool isFirstTimeLogin)
        {
            IsFirstTimeLogin = isFirstTimeLogin;
            ChangeUserPasswordLabel.Content = "Please change your password";

        }
        private void MethodChosen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MethodChosen.SelectedItem != null)
            {
                next_btn.IsEnabled = true; 
            }
        }
        private void next_btn_Click(object sender, RoutedEventArgs e)
        {

            FirstPanel.Visibility = Visibility.Collapsed;
            SecondPanel.Visibility = Visibility.Visible;
            ThirdPanel.Visibility = Visibility.Collapsed;
            FourthPanel.Visibility = Visibility.Collapsed;
            if (MethodChosen.Text == "Email") StatusValue.Tag = "Input your email";
            else StatusValue.Tag = "Input your phone number";
            StatusValue.Text = string.Empty;
        }
        private async void PhoneNumberMethod()
        {
            StatusValue.Text = StatusValue.Text.Substring(1);
            Random random = new Random();
            secretRandomNumber = (random.Next(100000, 999999)).ToString();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://cloudapi.plasgate.com/api/send");
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("855" + StatusValue.Text), "to");
            content.Add(new StringContent("SMS Info"), "sender");
            content.Add(new StringContent("OTP code: " + secretRandomNumber), "content");
            content.Add(new StringContent("kimchhengchhim@gmail.com"), "username");
            content.Add(new StringContent("098*123@$"), "password");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            MessageBox.Show("Please wait", "Inform", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void yes_btn_Click(object sender, RoutedEventArgs e)
        {
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            bool isEmail = Regex.IsMatch(StatusValue.Text, pattern);
            if (MethodChosen.Text == "Phone number") isEmail = true;
            if (isEmail)
            {
                if (MethodChosen.Text.Length == 0) return;
                if (StatusValue.Text.Length == 0)
                {
                    MessageBox.Show("Fill Field");
                    return;
                }
                next_btn.IsEnabled = false;
                cancel_btn.IsEnabled = false;
                statusResult.Content = string.Empty;
                MessageBox.Show("Please wait.......");
                Random random = new Random();
                secretRandomNumber = (random.Next(100000, 999999)).ToString();
                using (IBC_STOREIIEntities database = new IBC_STOREIIEntities())
                {
                    if (MethodChosen.Text == "Email")
                    {
                        foreach (var user in database.Users)
                        {
                            if (user.Email == StatusValue.Text.ToLower())
                            {
                                username = user.Username;
                                if (MessageBox.Show("Is '" + user.Username + "' your account?", "Account Verification", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    MessageBox.Show("Verification code has been sent to your email. Please wait.......");

                                    var smtpClient = new SmtpClient("smtp-mail.outlook.com")
                                    {
                                        Port = 587,
                                        Credentials = new NetworkCredential("anikjanas168@outlook.com", "Aptest123"),
                                        EnableSsl = true,
                                    };
                                    MailMessage mailMessage = new MailMessage();
                                    mailMessage.From = new MailAddress("anikjanas168@outlook.com", "IBC BOOK store");
                                    mailMessage.Subject = "Verification code";
                                    mailMessage.Body = "OTP Code: " + secretRandomNumber;

                                    mailMessage.To.Add(StatusValue.Text);
                                    smtpClient.Send(mailMessage);

                                    FirstPanel.Visibility = Visibility.Collapsed;
                                    SecondPanel.Visibility = Visibility.Collapsed;
                                    ThirdPanel.Visibility = Visibility.Visible;
                                    FourthPanel.Visibility = Visibility.Collapsed;
                                }
                                isEmailFound = true;
                                break;
                            }
                        }
                        if (isEmailFound != true)
                        {
                            statusResult.Content = "Your email does not exist";
                            statusResult.Foreground = Brushes.Red;
                        }

                    }
                    else if (MethodChosen.Text == "Phone number")
                    {
                        StatusValue.Text = StatusValue.Text.Trim();
                        if (StatusValue.Text.IndexOf(' ') != -1 || StatusValue.Text.Length <= 8)
                        {
                            MessageBox.Show("Check your phone number again", "Inform", MessageBoxButton.OK, MessageBoxImage.Error);
                            next_btn.IsEnabled = true;
                            cancel_btn.IsEnabled = true;
                            return;
                        }
                        if (!StatusValue.Text.StartsWith("0"))
                        {
                            MessageBox.Show("There is no zero number in the beginning of phone number", "Inform", MessageBoxButton.OK, MessageBoxImage.Error);
                            next_btn.IsEnabled = true;
                            cancel_btn.IsEnabled = true;
                            return;
                        }
                        foreach (var user in database.Users)
                        {
                            if (user.PhoneNumber == StatusValue.Text)
                            {
                                if (MessageBox.Show("Is '" + user.Username + "' your account?", "Account Verification", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    PhoneNumberMethod();
                                    FirstPanel.Visibility = Visibility.Collapsed;
                                    SecondPanel.Visibility = Visibility.Collapsed;
                                    ThirdPanel.Visibility = Visibility.Visible;
                                    FourthPanel.Visibility = Visibility.Collapsed;
                                }
                                isPhoneNumberFound = true;
                                break;
                            }
                        }
                        if (isPhoneNumberFound != true) MessageBox.Show("Check your phone number again");
                    }
                    isEmailFound = false;
                    isPhoneNumberFound = false;

                }
                next_btn.IsEnabled = true;
                cancel_btn.IsEnabled = true;

            }
            else
            {
                statusResult.Content = "Please enter a correct email";
                statusResult.Foreground = Brushes.Yellow;
            }
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            statusResult.Content = string.Empty;
            FirstPanel.Visibility = Visibility.Visible;
            SecondPanel.Visibility = Visibility.Collapsed;
            ThirdPanel.Visibility = Visibility.Collapsed;
            FourthPanel.Visibility = Visibility.Collapsed;
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            FirstPanel.Visibility = Visibility.Collapsed;
            SecondPanel.Visibility = Visibility.Visible;
            ThirdPanel.Visibility = Visibility.Collapsed;
            FourthPanel.Visibility = Visibility.Collapsed;
        }

        private void continue_btn_Click(object sender, RoutedEventArgs e)
        {
            if (OPT_tb.Text == secretRandomNumber)
            {

                FirstPanel.Visibility = Visibility.Collapsed;
                SecondPanel.Visibility = Visibility.Collapsed;
                ThirdPanel.Visibility = Visibility.Collapsed;
                FourthPanel.Visibility = Visibility.Visible;
                ChangeUserPasswordLabel.Content = string.Format("Change {0} Password", username);
            }
            else MessageBox.Show("Incorrect code. Please try again");


        }

        private void StatusValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (MethodChosen.Text == "Phone number")
                e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccessfullyChanged = false;
            bool has6Digits = false;
            if (new_pass_b.Password.Length < 6)
            {
                has6Digits = false;

            }
            else
            {
                has6Digits = true;
            }

            if (new_pass_b.Password == confirm_pass_b.Password && has6Digits == true)
            {
                if (IsFirstTimeLogin == true)
                {
                    ChangeFirstTimePassword(username, new_pass_b.Password);
                    this.Close();
                    return;
                }
                using (IBC_STOREIIEntities database = new IBC_STOREIIEntities())
                {
                    if (MethodChosen.Text == "Email")
                    {
                        foreach (var user in database.Users)
                        {
                            if (user.Email == StatusValue.Text.ToLower())
                            {
                                user.Password = new_pass_b.Password;
                                isSuccessfullyChanged = true;
                                break;
                            }
                        }
                    }
                    else if (MethodChosen.Text == "Phone number")
                    {
                        foreach (var user in database.Users)
                        {
                            if (user.PhoneNumber == "0" + StatusValue.Text)
                            {
                                user.Password = new_pass_b.Password;
                                isSuccessfullyChanged = true;
                                break;
                            }
                        }
                    }
                    database.SaveChanges();
                }
                if (isSuccessfullyChanged)
                {
                    isSuccessfullyChanged = false;
                    MessageBox.Show("Your account password has been changed");
                    Window1 main = new Window1();
                    main.Show();
                    Close();
                }
            }
            else
            {
                if (has6Digits == false)
                {
                    MessageBox.Show("Password must be at least 6 characters");
                }
                else if (new_pass_b.Password != confirm_pass_b.Password)
                    MessageBox.Show("Both of them are not matched");
            }

        }

        public void ChangeFirstTimePassword(string username1, string newpassword)
        {
            username = username1;
            using (IBC_STOREIIEntities database = new IBC_STOREIIEntities())
            {
                User user1 = new User();
                foreach (var user in database.Users)
                {
                    if (user.Username == username)
                    {
                        user1 = user;   
                        break;
                    }
                }

                user1.Password = newpassword;
                database.SaveChanges();
                MessageBox.Show(user1.Username + " Password saved successfully");

                Window1 login = new Window1();
                login.Show();


            }
        }

        internal void SetUsername(string userName)
        {
            username = userName;
        }
    }
}
