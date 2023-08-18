using IBC_Management_System.classdata;
using IBC_Management_System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer_Sale_Form : Window
    {
        static SqlConnection conn = null;
        public List<Product_Class_Invoice> Product_In_Invoice { get; set; }
        public Customer_Class customer { get; set; }
        public int UserID { get; set; }
        public int BranchID { get; set; }
        public string Usernane { get; set; }
        public Customer_Sale_Form()
        {
            InitializeComponent();
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["IBC_STOREIIEntities1"].ConnectionString;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Sales_Form sales = new Sales_Form();
            sales.Product_In_Invoice = Product_In_Invoice;
            sales.customer = customer;
            sales.BranchID = BranchID;
            sales.UserId = UserID;
            sales.UserName = Usernane;
            sales.Show();
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Closeapp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void createuser_Click(object sender, RoutedEventArgs e)
        {
            label.Text = "Create Customer";
            AddcustomerUi.Visibility = Visibility.Visible;
            SearchUI.Visibility = Visibility.Collapsed;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            label.Text = "Search Customer";
            SearchUI.Visibility = Visibility.Visible;
            AddcustomerUi.Visibility = Visibility.Collapsed;
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            Name_Textbox.Text = string.Empty;
            Phonenb_textbox.Text = string.Empty;
            Email_Textbox.Text = string.Empty;
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            if ((Phonenb_textbox.Text != "" || Email_Textbox.Text != "") && Name_Textbox.Text != "")
            {
                if (Phonenb_textbox.Text != "" && Email_Textbox.Text == "")
                {
                    if (int.TryParse(Phonenb_textbox.Text, out _))
                    {
                        List<Customer_Class> customers = Customer_Search(Phonenb_textbox.Text, null);
                        if (customers.Count == 0)
                        {
                            Customer_Create(Name_Textbox.Text, Phonenb_textbox.Text, null);
                            MessageBox.Show("Account has been create");
                            customers = Customer_Search(Phonenb_textbox.Text, null);
                            customer = customers[0];
                            Sales_Form sales = new Sales_Form();
                            sales.Product_In_Invoice = Product_In_Invoice;
                            sales.customer = customer;
                            sales.BranchID = BranchID;
                            sales.UserId = UserID;
                            sales.UserName = Usernane;
                            sales.Show();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Account already exist");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Phonenumber can not has any letter in it");
                    }
                }
                else if (Email_Textbox.Text != "" && Phonenb_textbox.Text == "")
                {
                    if (Email_Textbox.Text.Contains("@gmail.com"))
                    {
                        List<Customer_Class> customers = Customer_Search(null, Email_Textbox.Text);
                        if (customers.Count == 0)
                        {
                            Customer_Create(Name_Textbox.Text, null, Email_Textbox.Text);
                            MessageBox.Show("Account has been create");
                            customers = Customer_Search(null, Email_Textbox.Text);
                            customer = customers[0];
                            Sales_Form sales = new Sales_Form();
                            sales.Product_In_Invoice = Product_In_Invoice;
                            sales.customer = customer;
                            sales.BranchID = BranchID;
                            sales.UserId = UserID;
                            sales.UserName = Usernane;
                            sales.Show();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Account already exist");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email must end with @gmail.com");
                    }
                }
                else
                {
                    if (int.TryParse(Phonenb_textbox.Text, out _))
                    {
                        List<Customer_Class> customersByPhone = Customer_Search(Phonenb_textbox.Text, null);
                        List<Customer_Class> customersByEmail = Customer_Search(null, Email_Textbox.Text);

                        if (customersByPhone.Count > 0 || customersByEmail.Count > 0)
                        {
                            MessageBox.Show("Phonenumber or Email already used");
                        }
                        else
                        {
                            if (Email_Textbox.Text.Contains("@gmail.com"))
                            {
                                Customer_Create(Name_Textbox.Text, Phonenb_textbox.Text, Email_Textbox.Text);
                                List<Customer_Class> customers = Customer_Search(Phonenb_textbox.Text, Email_Textbox.Text);
                                customer = customers[0];
                                MessageBox.Show("Account has been created");
                                Sales_Form sales = new Sales_Form();
                                sales.Product_In_Invoice = Product_In_Invoice;
                                sales.customer = customer;
                                sales.BranchID = BranchID;
                                sales.UserId = UserID;
                                sales.UserName = Usernane;
                                sales.Show();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Email must end with @gmail.com");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Phonenumber cannot have any letters in it");
                    }
                }
            }
            else
            {
                MessageBox.Show("You need Name and (Phonenumber or Email) to create customer");
            }
        }
        public static List<Customer_Class> Customer_Search(string PhoneNumber, string Email)
        {
            List<Customer_Class> list = new List<Customer_Class>();
            SqlDataAdapter da = null;
            DataSet ds = null;
            if (PhoneNumber != null && Email == null)
            {
                da = new SqlDataAdapter("SELECT * from Customers WHERE Phonenumber = @Phonenumber", conn);
                SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                da.SelectCommand.Parameters.Add("@Phonenumber", SqlDbType.NVarChar).Value = PhoneNumber;
            }
            else if (Email != null && PhoneNumber == null)
            {
                da = new SqlDataAdapter("SELECT * from Customers WHERE Email = @Email", conn);
                SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                da.SelectCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
            }
            else
            {
                da = new SqlDataAdapter("SELECT * from Customers WHERE Email = @Email AND Phonenumber = @Phonenumber", conn);
                SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                da.SelectCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                da.SelectCommand.Parameters.Add("@Phonenumber", SqlDbType.NVarChar).Value = PhoneNumber;
            }
            ds = new DataSet();
            da.Fill(ds);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Customer_Class temp = new Customer_Class();
                if (ds.Tables[0].Rows[i]["Id"] != System.DBNull.Value)
                {
                    temp.Id = (int)ds.Tables[0].Rows[i]["Id"];
                }
                else
                {
                    temp.Id = 0;
                }
                if (ds.Tables[0].Rows[i]["Id"] != System.DBNull.Value)
                {

                    temp.Name = (string)ds.Tables[0].Rows[i]["Name"];
                }
                else
                {
                    temp.Name = null;
                }
                if (ds.Tables[0].Rows[i]["Phonenumber"] != System.DBNull.Value) temp.PhoneNumber = (string)ds.Tables[0].Rows[i]["Phonenumber"];
                else temp.PhoneNumber = null;
                if (ds.Tables[0].Rows[i]["Email"] != System.DBNull.Value) temp.Email = (string)ds.Tables[0].Rows[i]["Email"];
                else temp.Email = null;
                list.Add(temp);
            }
            return list;
        }
        public static void Customer_Create(string Name, string PhoneNumber, string Email)
        {

            // Create a new DataTable and add a row with the data
            DataTable dataTable = new DataTable("Customers");
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));

            DataRow newRow = dataTable.NewRow();
            newRow["Name"] = Name;
            newRow["PhoneNumber"] = string.IsNullOrEmpty(PhoneNumber) ? (object)DBNull.Value : PhoneNumber;
            newRow["Email"] = string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email;
            dataTable.Rows.Add(newRow);

            // Create a new SqlDataAdapter and insert the data
            string insertQuery = "INSERT INTO Customers (Name, PhoneNumber, Email) VALUES (@Name, @PhoneNumber, @Email);";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                dataAdapter.InsertCommand = new SqlCommand(insertQuery, conn);
                dataAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.VarChar, 100, "Name");
                dataAdapter.InsertCommand.Parameters.Add("@PhoneNumber", SqlDbType.VarChar, 20, "PhoneNumber");
                dataAdapter.InsertCommand.Parameters.Add("@Email", SqlDbType.VarChar, 100, "Email");

                dataAdapter.Update(dataTable);
            }
        }

        private void Email_Checkbox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void phonenb_Checkbox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (Search_Textbox.Text != "")
            {
                List<Customer_Class> customers = null;
                if (Email_Checkbox.IsChecked == true)
                {
                    if (Search_Textbox.Text.ToLower().Contains("@gmail.com"))
                    {
                        customers = Customer_Search(null, Search_Textbox.Text);
                    }
                    else
                    {
                        MessageBox.Show("Email must end with @gmail.com");
                    }
                }
                else
                {
                    if (int.TryParse(Search_Textbox.Text, out _))
                    {
                        customers = Customer_Search(Search_Textbox.Text, null);
                    }
                    else
                    {
                        MessageBox.Show("Phonenumber can not has any letter in it");
                    }
                }
                if (customers != null)
                {
                    if (customers.Count != 0)
                    {
                        customer = customers[0];
                        Sales_Form sale = new Sales_Form();
                        sale.Product_In_Invoice = Product_In_Invoice;
                        sale.customer = customer;
                        sale.BranchID = BranchID;
                        sale.UserId = UserID;
                        sale.UserName = Usernane;
                        sale.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Account does not Exist");
                    }
                }
            }
            else
            {
                if (Email_Checkbox.IsChecked == true)
                {
                    MessageBox.Show("Input Email");
                }
                else
                {
                    MessageBox.Show("Input Phonenumber");
                }
            }
        }

        private void phonenb_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (Email_Checkbox.IsChecked == true)
            {
                phonenb_Checkbox.IsChecked = true;
                Email_Checkbox.IsChecked = false;
            }
            else
            {
                phonenb_Checkbox.IsChecked = false;
                Email_Checkbox.IsChecked = true;
            }
        }

        private void Email_Checkbox_Click(object sender, RoutedEventArgs e)
        {

            if (phonenb_Checkbox.IsChecked == true)
            {
                Email_Checkbox.IsChecked = true;
                phonenb_Checkbox.IsChecked = false;
            }
            else
            {
                Email_Checkbox.IsChecked = false;
                phonenb_Checkbox.IsChecked = true;
            }
        }
    }
    public class Customer_Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
