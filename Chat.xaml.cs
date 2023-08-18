using IBC_Management_System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        List<string> users = new List<string>();
        string strSend = string.Empty;
        string username;
        string role = string.Empty;
        IBC_STOREIIEntities db = new IBC_STOREIIEntities();



        public Chat(string username)
        {
            //Window1.Get
            //username = mainWindow.GetUsername();
            GetRoleOfUser(username);
            InitializeComponent();
            this.username = username;
        }
        public void GetRoleOfUser(string username)
        {
            var users = db.Users.Where(x => x.Username == username).FirstOrDefault();
            role = users.Role.Description;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Admin mainWindow = new Admin();
            mainWindow.Show();
            this.Hide();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TextChat.Text))
            {

                try
                {

                    if (s.Connected)
                    {
                        string strSend = string.Empty;
                        strSend = TextChat.Text;
                        s.Send(Encoding.ASCII.GetBytes(strSend));
                        TextChat.Clear();
                        string now = DateTime.Now.ToString();
                        if (strSend[0] == '?')
                        {
                            //move this to the end of the string
                            strSend = strSend.Substring(1);
                            strSend += '?';
                        }
                        if (strSend[strSend.Length-1] =='?')
                                                   {
                           // move this to the beginning of the string
                            strSend = strSend.Substring(0, strSend.Length - 1);
                            strSend = '?' + strSend;
                        }
                        
                        
                        ListChatUser.Items.Add("Sent " + DateTime.Now.ToString() + Environment.NewLine + strSend);
                        ListChatUser.Items[ListChatUser.Items.Count - 1] = new ListViewItem { Content = ListChatUser.Items[ListChatUser.Items.Count - 1], FlowDirection = FlowDirection.RightToLeft };
                       // ListChatUser.Items[ListChatUser.Items.Count - 1] = new ListViewItem { Content = "Sent " + DateTime.Now.ToString() + Environment.NewLine + strSend, HorizontalContentAlignment = HorizontalAlignment.Right };
                  
                        ListChatUser.ScrollIntoView(ListChatUser.Items[ListChatUser.Items.Count - 1]);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (s != null)
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);
            try
            {
                s.Connect(ep);
                if (s.Connected)
                {
                    s.Send(Encoding.ASCII.GetBytes(username));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Please run server first or try checking IP and port of server");
                Environment.Exit(1);
            }
            Thread thread1 = new Thread(GetMessages);
            thread1.IsBackground = true;
            thread1.Start();

        }

        private void GetMessages()
        {
            while (true)
            {
                try
                {

                    byte[] buffer = new byte[1024];
                    int i = s.Receive(buffer);
                    string str = Encoding.ASCII.GetString(buffer, 0, i);
                    if (str.Contains("?gm ds AddUser"))
                    {

                        AddUser(str);
                    }
                    else if (str.Contains("?gm ds RemoveUser"))
                    {

                        RemoveUser(str, users, ListViewUser, ListChatUser);
                    }
                    else if (str == "?gm ds MakeAnnouncement")
                    {

                        MessageBoxResult result = MessageBox.Show("Admin just made an announcement. Do you want to open it?", "Announcement", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Button_Click_1(result, null);
                        }
                        else
                        {

                        }
                    }
                    else if (str == "")
                    {
                    }
                    else
                    {
                        string[] messageRecieved = str.Split(':');
                        if (messageRecieved[0] != username && messageRecieved[1] != " ")
                            this.Dispatcher.Invoke(() => ListChatUser.Items.Add("[" + DateTime.Now.ToString() + "] " + Environment.NewLine + str));
                        this.Dispatcher.Invoke(() => ListChatUser.ScrollIntoView(ListChatUser.Items[ListChatUser.Items.Count - 1]));



                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error1");
                }
            }
        }
        private void TextChat_KeyUP(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
        private void RemoveUser(string str, List<string> users, ListView ListViewUser, ListView ListChatUser)
        {
            string[] commands = str.Split(';');
            foreach (var command in commands)
            {
                try
                {
                    string[] strArr = command.Split('(');
                    string name = strArr[1].Split(')')[0];

                    if (users.Any(x => x == name))
                    {
                        users.Remove(name);
                        this.Dispatcher.Invoke(() => ListViewUser.Items.Remove(name));
                        this.Dispatcher.Invoke(() => ListChatUser.Items.Add(string.Format("[" + DateTime.Now.ToString() + "] " + Environment.NewLine + "{0} went offline", name)));
                        this.Dispatcher.Invoke(() => ListChatUser.Items[ListChatUser.Items.Count - 1] = new ListViewItem { Content = ListChatUser.Items[ListChatUser.Items.Count - 1], HorizontalAlignment = HorizontalAlignment.Center });

                    }
                }
                catch
                {

                }
            }
        }
        private void AddUser(string str)
        {
            string[] commands = str.Split(';');
            foreach (var command in commands)
            {
                try
                {
                    string[] strArr = command.Split('(');
                    string name = strArr[1].Split(')')[0];

                    if (!users.Any(x => x == name))
                    {
                        users.Add(name);
                        this.Dispatcher.Invoke(() => ListViewUser.Items.Add(name));
                        this.Dispatcher.Invoke(() => ListChatUser.Items.Add(string.Format("[" + DateTime.Now.ToString() + "] " + Environment.NewLine + "{0} came online", name)));
                        this.Dispatcher.Invoke(() => ListChatUser.Items[ListChatUser.Items.Count - 1] = new ListViewItem { Content = ListChatUser.Items[ListChatUser.Items.Count - 1], HorizontalAlignment = HorizontalAlignment.Center });
                    }
                }
                catch
                {

                }
            }
        }
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(int hwnd);
        [DllImport("user32.dll")]

        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (role == "admin")
            {

                AnnoucementForm form = new AnnoucementForm(true);
                form.Show();

            }
            else
            {


                Process[] processes = Process.GetProcessesByName("AnnouncementPanel");
                if (processes.Length > 0)
                {
                    IntPtr handle = processes[0].MainWindowHandle;
                    processes[0].Kill();
                }
                AnnoucementForm form = new AnnoucementForm(false);
                form.SetIsAdmin(false);
                form.Show();
            }
        }

        private void TextChat_KeyUp(object sender, KeyEventArgs e)
        {

        }

    }
}
