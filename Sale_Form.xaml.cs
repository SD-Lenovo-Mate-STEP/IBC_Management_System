using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mail;
using System.Net;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Paragraph = iTextSharp.text.Paragraph;
using Path = System.IO.Path;
using IBC_Management_System.classdata;

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales_Form : Window
    {
        static SqlConnection conn = null;
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BranchID { get; set; }
        public List<Product_Class_Invoice> Product_In_Invoice { get; set; } = new List<Product_Class_Invoice>();
        public Customer_Class customer { get; set; } = null;
        ObservableCollection<Product_Class_Sale> product_Output { get; set; } = new ObservableCollection<Product_Class_Sale>();
        ObservableCollection<Product_Class_Invoice> product_Sale_Output { get; set; } = new ObservableCollection<Product_Class_Invoice>();
        private List<Invoice_Print_Sale> Temp_PDF_Sale { get; set; } = new List<Invoice_Print_Sale>();
        private Customer_Class Temp_Customer { get; set; } = new Customer_Class();


        public Sales_Form()
        {
            InitializeComponent();

            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["IBC_STOREIIEntities1"].ConnectionString;


            List<Category_Class_Sale> category_Class_Sales = Search_Category();
            for (int i = 0; i < category_Class_Sales.Count; i++)
            {
                Category_Button_User category_Button_User = new Category_Button_User();
                category_Button_User.Width = 300;
                category_Button_User.Height = 60;
                category_Button_User.Category_Class_Sale = category_Class_Sales[i];
                category_Button_User.Button_Click_Event += uc_Button_Click_Event;
                CateGory_Panel.Children.Add(category_Button_User);
            }
        }

        private void uc_Button_Click_Event(object sender, Category_Button_User.Button_Click_Invoke e)
        {
            UserControl_Product_Show.Children.Clear();
            product_Output.Clear();
            string textbox = textbox_search.Text;
            List<Product_Class_Sale> products = Product_Search("", BranchID, true, e.Category_Class_Sale.Name);
            if (products.Count != 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    Product_Show_UserControl product_UserControl = new Product_Show_UserControl();
                    product_UserControl.Product = products[i];
                    product_Output.Add(products[i]);
                    product_UserControl.Button_Click_Event += Product_UC_Button_Click_Event;
                    UserControl_Product_Show.Children.Add(product_UserControl);
                }
                Product_DataGrid.ItemsSource = product_Output;
            }
        }

        private void Product_UC_Button_Click_Event(object sender, Product_Show_UserControl.Button_Click_Invoke e)
        {
            Quantity_Sale_Product quantity_Sale_Product = new Quantity_Sale_Product();
            quantity_Sale_Product.Product_In_Invoice = Product_In_Invoice;
            quantity_Sale_Product.customer = customer;
            quantity_Sale_Product.Product_Class_Sale = e.Product;
            quantity_Sale_Product.UserID = UserId;
            quantity_Sale_Product.BranchID = BranchID;
            quantity_Sale_Product.Username = UserName;
            quantity_Sale_Product.Texts = "";
            quantity_Sale_Product.Show();
            Close();
        }

        private void Closeapp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Window1 mainWindow = new Window1();
            mainWindow.Show();
            Close();
        }


        private void search_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Product_Show.Children.Clear();
            product_Output.Clear();
            string textbox = textbox_search.Text;
            List<Product_Class_Sale> products = null;
            if (int.TryParse(textbox, out int a))
            {
                products = Product_Search(textbox, BranchID, false);
            }
            else
            {
                products = Product_Search(textbox, BranchID);
            }
            if (products != null)
            {
                if (products.Count == 0)
                {
                    MessageBox.Show("Product not Found");
                }
                if (products.Count == 1)
                {
                    Quantity_Sale_Product quantity_Sale_Product = new Quantity_Sale_Product();
                    quantity_Sale_Product.Product_In_Invoice = Product_In_Invoice;
                    quantity_Sale_Product.customer = customer;
                    quantity_Sale_Product.Product_Class_Sale = products[0];
                    quantity_Sale_Product.UserID = UserId;
                    quantity_Sale_Product.BranchID = BranchID;
                    quantity_Sale_Product.Username = UserName;
                    quantity_Sale_Product.Texts = "";
                    quantity_Sale_Product.Show();
                    Close();
                }
                else
                {
                    for (int i = 0; i < products.Count; i++)
                    {
                        Product_Show_UserControl product_UserControl = new Product_Show_UserControl();
                        product_UserControl.Product = products[i];
                        product_Output.Add(products[i]);
                        product_UserControl.Button_Click_Event += Product_UC_Button_Click_Event;
                        UserControl_Product_Show.Children.Add(product_UserControl);
                    }
                    Product_DataGrid.ItemsSource = product_Output;
                }
            }

        }
        public static List<Product_Class_Sale> Product_Search(string Name, int BranchID, bool IsName = true, string Categories = "All")
        {

            SqlDataAdapter da = null;
            DataSet ds = null;
            List<Product_Class_Sale> products = new List<Product_Class_Sale>();
            if (IsName == true)
            {
                if (Categories != "All")
                {
                    da = new SqlDataAdapter("SELECT p.Name,p.Id,p.BarCode,p.Picture,p.Price,c.Name AS Category, S.Quantity AS Stock FROM Products as p, Categories as C, Stocks as S WHERE p.Name LIKE @Name + '%' AND p.CategoryId = c.Id AND c.Name = @Categories AND S.BranchId = @BranchID AND S.ProductId = p.Id", conn);
                    SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                    da.SelectCommand.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                    da.SelectCommand.Parameters.Add("@Categories", SqlDbType.NVarChar).Value = Categories;
                    da.SelectCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = BranchID;
                }
                else
                {
                    da = new SqlDataAdapter("SELECT p.Name,p.Id,p.BarCode,p.Picture,p.Price, S.Quantity AS Stock FROM Products as p, Stocks as S WHERE p.Name LIKE @Name + '%'  AND S.BranchId = @BranchID AND S.ProductId = p.Id", conn);
                    SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                    da.SelectCommand.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                    da.SelectCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = BranchID;
                }
            }
            else
            {
                if (Categories != "All")
                {
                    da = new SqlDataAdapter("SELECT p.Name,p.Id,p.BarCode,p.Picture,p.Price,c.Name AS Category, S.Quantity AS Stock FROM Products as p, Categories as C, Stocks as S WHERE p.BarCode LIKE @Barcode + '%' AND p.CategoryId = c.Id AND c.Name = @Categories AND S.BranchId = @BranchID AND S.ProductId = p.Id", conn);
                    SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                    da.SelectCommand.Parameters.Add("@Barcode", SqlDbType.NVarChar).Value = Name;
                    da.SelectCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = BranchID;
                    da.SelectCommand.Parameters.Add("@Categories", SqlDbType.NVarChar).Value = Categories;
                }
                else
                {
                    da = new SqlDataAdapter("SELECT p.Name,p.Id,p.BarCode,p.Picture,p.Price, S.Quantity AS Stock FROM Products as p, Stocks as S WHERE p.BarCode LIKE @BarCode + '%'  AND S.BranchId = @BranchID AND S.ProductId = p.Id", conn);
                    SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                    da.SelectCommand.Parameters.Add("@Barcode", SqlDbType.NVarChar).Value = Name;
                    da.SelectCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = BranchID;
                }
            }
            ds = new DataSet();
            da.Fill(ds);


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Product_Class_Sale temp = new Product_Class_Sale();
                if (ds.Tables[0].Rows[i]["Name"] != System.DBNull.Value)
                {
                    temp.Name = (string)ds.Tables[0].Rows[i]["Name"];
                }
                else
                {
                    temp.Name = null;
                }
                if (ds.Tables[0].Rows[i]["Stock"] != System.DBNull.Value)
                {
                    temp.Stock = (int)ds.Tables[0].Rows[i]["Stock"];
                }
                else
                {
                    temp.Stock = 0;
                }
                if (ds.Tables[0].Rows[i]["BarCode"] != System.DBNull.Value)
                {
                    temp.Barcode = (string)ds.Tables[0].Rows[i]["BarCode"];
                }
                else
                {
                    temp.Barcode = null;
                }
                if (ds.Tables[0].Rows[i]["Id"] != System.DBNull.Value)
                {
                    temp.Id = (int)ds.Tables[0].Rows[i]["Id"];
                }
                else
                {
                    temp.Id = 0;
                }
                if (ds.Tables[0].Rows[i]["Picture"] != System.DBNull.Value)
                {
                    temp.Picture = (byte[])ds.Tables[0].Rows[i]["Picture"];
                }
                else
                {
                    temp.Picture = null;
                }
                if (ds.Tables[0].Rows[i]["Price"] != System.DBNull.Value)
                {
                    temp.Price = (decimal)ds.Tables[0].Rows[i]["Price"];
                }
                else
                {
                    temp.Price = 0;
                }
                if (Categories != "All")
                {
                    if (ds.Tables[0].Rows[i]["Category"] != System.DBNull.Value)
                    {
                        temp.Category = (string)ds.Tables[0].Rows[i]["Category"];
                    }
                    else
                    {
                        temp.Category = null;
                    }
                }
                else
                {
                    temp.Category = null;
                }
                products.Add(temp);
            }
            Console.WriteLine("Complete");
            return products;
        }
        private static List<Category_Class_Sale> Search_Category()
        {
            List<Category_Class_Sale> list = new List<Category_Class_Sale>();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * from Categories", conn);
                SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Category_Class_Sale temp = new Category_Class_Sale();
                    temp.id = (int)ds.Tables[0].Rows[i]["id"];
                    temp.Name = (string)ds.Tables[0].Rows[i]["Name"];
                    list.Add(temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (customer != null)
            {
                Customer_Detail_Label.Text = "Customer : " + customer.Name;
                Customer_Detail_Label.Visibility = Visibility.Visible;
            }
            else
            {
                Customer_Detail_Label.Visibility = Visibility.Hidden;
                //Customer_Detail_Label.Text = "Customer : ";
            }
            Product_DataGrid.ItemsSource = product_Output;
            List<Product_Class_Sale> products = Product_Search("", BranchID, true);
            if (products.Count != 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    Product_Show_UserControl product_UserControl = new Product_Show_UserControl();
                    product_UserControl.Product = products[i];
                    product_Output.Add(products[i]);
                    product_UserControl.Button_Click_Event += Product_UC_Button_Click_Event;
                    UserControl_Product_Show.Children.Add(product_UserControl);
                }
                //Product_DataGrid.Columns[4].Visibility = Visibility.Hidden;
            }
            Sale_List_DataGrid.ItemsSource = product_Sale_Output;
            if (Product_In_Invoice.Count != 0)
            {
                decimal total = 0;
                for (int i = 0; i < Product_In_Invoice.Count; i++)
                {
                    total += Product_In_Invoice[i].Quantity * Product_In_Invoice[i].Price;
                    product_Sale_Output.Add(Product_In_Invoice[i]);
                }
                Total.Text = $"{total}";
                Sale_List_DataGrid.Columns[4].Visibility = Visibility.Hidden;
                Sale_List_DataGrid.Columns[0].Width = 50;
                Sale_List_DataGrid.Columns[1].Width = 200;
                Sale_List_DataGrid.Columns[2].Width = 75;
                Sale_List_DataGrid.Columns[3].Width = 70;
                Sale_List_DataGrid.Columns[5].Width = 200;
                Sale_List_DataGrid.Columns[6].Width = 200;
                Totalitem.Text = $"{Product_In_Invoice.Count}";
            }
            else
            {
                Sale_List_DataGrid.Visibility = Visibility.Hidden;
            }
        }
        private void Addcustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer_Sale_Form customers = new Customer_Sale_Form();
            customers.customer = customer;
            customers.Product_In_Invoice = Product_In_Invoice;
            customers.UserID = UserId;
            customers.BranchID = BranchID;
            customers.Usernane = UserName;
            customers.Show();
            Close();
        }

        private void Product_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Product_Class_Sale selectedItem = (Product_Class_Sale)Product_DataGrid.SelectedItem;

                Quantity_Sale_Product quantity_Sale_Product = new Quantity_Sale_Product();
                quantity_Sale_Product.Product_In_Invoice = Product_In_Invoice;
                quantity_Sale_Product.customer = customer;
                quantity_Sale_Product.UserID = UserId;
                quantity_Sale_Product.BranchID = BranchID;
                quantity_Sale_Product.Username = UserName;
                quantity_Sale_Product.Product_Class_Sale = selectedItem;
                quantity_Sale_Product.Texts = "";
                quantity_Sale_Product.Show();
                Close();
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (customer != null)
            {
                if (customer.Id != 0)
                {
                    if (Customer_Paid.Text != "")
                    {
                        if (double.TryParse(Customer_Paid.Text, out double Paid))
                        {
                            if (Paid >= Convert.ToDouble(Total.Text))
                            {
                                if (product_Sale_Output.Count != 0)
                                {
                                    DateTime dates = DateTime.Now;
                                    string date = dates.ToShortDateString() + " " + dates.ToShortTimeString();
                                    Invoice_Customer_Name_Label.Text = customer.Name;
                                    if (customer.PhoneNumber != null)
                                    {
                                        Invoice_Customer_Phone_Email_Label.Text = customer.PhoneNumber;
                                    }
                                    else
                                    {
                                        Invoice_Customer_Phone_Email_Label.Text = customer.Email;
                                    }
                                    List<Invoice_Print_Sale> products = new List<Invoice_Print_Sale>();
                                    int b = 1;
                                    foreach (var product in Product_In_Invoice)
                                    {
                                        Invoice_Print_Sale temp = new Invoice_Print_Sale();
                                        temp.Id = b;
                                        temp.Price = product.Price;
                                        temp.Total_Price = product.Price * product.Quantity;
                                        temp.Quantity = product.Quantity;
                                        temp.Name = product.Name;
                                        b++;
                                        products.Add(temp);
                                    }
                                    Temp_PDF_Sale = products;
                                    Temp_Customer = customer;
                                    Invoice_Print_DataGrid.ItemsSource = products;
                                    Invoice_Print_DataGrid.Columns[0].Width = 50;
                                    Invoice_Print_DataGrid.Columns[1].Width = 200;
                                    Invoice_Print_DataGrid.Columns[2].Width = 200;
                                    Invoice_Print_DataGrid.Columns[3].Width = 200;
                                    Invoice_Print_DataGrid.Columns[4].Width = 200;


                                    Invoice_Date_Label.Text = date;
                                    Invoice_Seller_Name_Label.Text = UserName;
                                    Invoice_Pay_Label.Text = Customer_Paid.Text;
                                    var a = Convert.ToDouble(Customer_Paid.Text) - Convert.ToDouble(Total.Text);
                                    Invoice_Money_Give_Back_Label.Text = $"{a}";
                                    Invoice_Seller_ID_Label.Text = $"{UserId}";
                                    Invoice_Total_Label.Text = Total.Text;
                                    Start_Printing_Grid.Visibility = Visibility.Visible;
                                    Back_Grid_Item.Visibility = Visibility.Hidden;
                                    Product_Detail_Show.Visibility = Visibility.Hidden;
                                    Invoice_Detail_Show.Visibility = Visibility.Visible;
                                    Customer_Paid.Text = "";


                                    Create_Invoice(dates.ToString(), Convert.ToDouble(Total.Text), customer.Id, UserId, BranchID);
                                    Invoice_Class_Sale Invoice = Search_Invoice(dates.ToString(), Convert.ToDouble(Total.Text), customer.Id, UserId, BranchID);
                                    for (int i = 0; i < Product_In_Invoice.Count; i++)
                                    {
                                        Sale_Create(Product_In_Invoice[i].Quantity, Product_In_Invoice[i].Id, Invoice.Id);
                                        Update_Stock(Product_In_Invoice[i].Id, BranchID, Product_In_Invoice[i].Quantity, conn);
                                    }


                                    Invoie_InvoiceID_Label.Text = $"{Invoice.Id}";
                                    Total.Text = $"{0}";
                                    Totalitem.Text = $"{0}";
                                    product_Sale_Output.Clear();
                                    Product_In_Invoice.Clear();
                                    customer = null;
                                    Customer_Detail_Label.Visibility = Visibility.Hidden;
                                }
                                else
                                {
                                    MessageBox.Show("There is not any item");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Customer paid must be higher than total");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Custoner paid incorrect format");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Customer paid unknown");
                    }
                }
                else
                {
                    MessageBox.Show("Customer Needed");
                }
            }
            else
            {
                MessageBox.Show("Customer Needed");
            }
        }
        static void Update_Stock(int ProductID, int BranchID, int Quantity, SqlConnection connection)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                string updateQuery = "UPDATE Stocks " +
                                     "SET Quantity = Quantity - @QuantityToUpdate " +
                                     "WHERE BranchID = @BranchID AND ProductID = @ProductID";

                command.CommandText = updateQuery;
                command.Parameters.AddWithValue("@QuantityToUpdate", Quantity);
                command.Parameters.AddWithValue("@BranchID", BranchID);
                command.Parameters.AddWithValue("@ProductID", ProductID);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
        static Invoice_Class_Sale Search_Invoice(string date, double total, int CustomerId, int UserId, int BranchId)
        {
            Invoice_Class_Sale Invoice = new Invoice_Class_Sale();
            //SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Invoices WHERE Total = @Total AND Date = @Date AND Userid = @Userid AND Branchid = @Branchid  AND Customerid = @Customerid;", conn);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Invoices WHERE Total = @Total AND DATEPART(YEAR, Date) = @Year AND DATEPART(MONTH, Date) = @Month AND DATEPART(DAY, Date) = @Day AND DATEPART(HOUR, Date) = @Hour AND DATEPART(MINUTE, Date) = @Minute AND DATEPART(SECOND, Date) = @Second AND Userid = @Userid AND Branchid = @Branchid  AND Customerid = @Customerid;\r\n", conn);

            // Add parameters in the correct order
            da.SelectCommand.Parameters.Add("@Total", SqlDbType.Decimal).Value = total;
            da.SelectCommand.Parameters.Add("@Year", SqlDbType.Int).Value = DateTime.Parse(date).Year;
            da.SelectCommand.Parameters.Add("@Month", SqlDbType.Int).Value = DateTime.Parse(date).Month;
            da.SelectCommand.Parameters.Add("@Day", SqlDbType.Int).Value = DateTime.Parse(date).Day;
            da.SelectCommand.Parameters.Add("@Hour", SqlDbType.Int).Value = DateTime.Parse(date).Hour;
            da.SelectCommand.Parameters.Add("@Minute", SqlDbType.Int).Value = DateTime.Parse(date).Minute;
            da.SelectCommand.Parameters.Add("@Second", SqlDbType.Int).Value = DateTime.Parse(date).Second;
            da.SelectCommand.Parameters.Add("@Userid", SqlDbType.Int).Value = UserId;
            da.SelectCommand.Parameters.Add("@Branchid", SqlDbType.Int).Value = BranchId;
            da.SelectCommand.Parameters.Add("@Customerid", SqlDbType.Int).Value = CustomerId;

            SqlCommandBuilder cmb = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count != 0)
            {
                Invoice.Id = (int)ds.Tables[0].Rows[0]["Id"];
                Invoice.Date = (DateTime)ds.Tables[0].Rows[0]["Date"];
                Invoice.Total = (decimal)ds.Tables[0].Rows[0]["Total"];
                Invoice.Branchid = (int)ds.Tables[0].Rows[0]["Branchid"];
                Invoice.Userid = (int)ds.Tables[0].Rows[0]["Userid"];
                Invoice.Customerid = (int)ds.Tables[0].Rows[0]["Customerid"];
            }
            return Invoice;
        }
        static void Create_Invoice(string date, double total, int CustomerId, int UserId, int BranchId)
        {
            DataTable dataTable = new DataTable("Invoices");
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Columns.Add("Total", typeof(double));
            dataTable.Columns.Add("CustomerId", typeof(int));
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("BranchId", typeof(int));

            DataRow newRow = dataTable.NewRow();
            newRow["Date"] = DateTime.Parse(date);
            newRow["Total"] = total;
            newRow["CustomerId"] = CustomerId;
            newRow["UserId"] = UserId;
            newRow["BranchId"] = BranchId;
            dataTable.Rows.Add(newRow);

            // Create a new SqlDataAdapter and insert the data
            string insertInvoiceQuery = "INSERT INTO Invoices (Date, Total, CustomerId, UserId, BranchId) VALUES (@Date, @Total, @CustomerId, @UserId, @BranchId)";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                dataAdapter.InsertCommand = new SqlCommand(insertInvoiceQuery, conn);
                dataAdapter.InsertCommand.Parameters.Add("@Date", SqlDbType.DateTime, 0, "Date");
                dataAdapter.InsertCommand.Parameters.Add("@Total", SqlDbType.Float, 0, "Total");
                dataAdapter.InsertCommand.Parameters.Add("@CustomerId", SqlDbType.Int, 0, "CustomerId");
                dataAdapter.InsertCommand.Parameters.Add("@UserId", SqlDbType.Int, 0, "UserId");
                dataAdapter.InsertCommand.Parameters.Add("@BranchId", SqlDbType.Int, 0, "BranchId");

                dataAdapter.Update(dataTable);
            }

        }
        static void Sale_Create(int quantity, int product_id, int invoice_id)
        {

            // Create a new DataTable and add a row with the data
            DataTable dataTable = new DataTable("Sales");
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("ProductId", typeof(int));
            dataTable.Columns.Add("InvoiceId", typeof(int));

            DataRow newRow = dataTable.NewRow();
            newRow["Quantity"] = quantity;
            newRow["ProductId"] = product_id;
            newRow["InvoiceId"] = invoice_id;
            dataTable.Rows.Add(newRow);

            // Create a new SqlDataAdapter and insert the data
            string insertSaleQuery = "INSERT INTO Sales (Quantity, ProductId, InvoiceId) VALUES (@Quantity, @ProductId, @InvoiceId)";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                dataAdapter.InsertCommand = new SqlCommand(insertSaleQuery, conn);
                dataAdapter.InsertCommand.Parameters.Add("@Quantity", SqlDbType.Int, 0, "Quantity");
                dataAdapter.InsertCommand.Parameters.Add("@ProductId", SqlDbType.Int, 0, "ProductId");
                dataAdapter.InsertCommand.Parameters.Add("@InvoiceId", SqlDbType.Int, 0, "InvoiceId");

                dataAdapter.Update(dataTable);
            }
        }

        private void Sale_List_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Product_Class_Invoice selectedItem = (Product_Class_Invoice)Sale_List_DataGrid.SelectedItem;

                Quantity_Sale_Product quantity_Sale_Product = new Quantity_Sale_Product();
                quantity_Sale_Product.Product_In_Invoice = Product_In_Invoice;
                quantity_Sale_Product.Delete_Product_Class = selectedItem;
                quantity_Sale_Product.customer = customer;
                quantity_Sale_Product.UserID = UserId;
                quantity_Sale_Product.BranchID = BranchID;
                quantity_Sale_Product.Username = UserName;
                quantity_Sale_Product.Texts = "Remove";
                quantity_Sale_Product.Show();
                Close();
            }
            catch
            {

            }
        }

        private void PrintPDF_Invoice_Button_Click(object sender, RoutedEventArgs e)
        {
            Print_PDF(Convert.ToInt32(Invoie_InvoiceID_Label.Text), Temp_PDF_Sale, Temp_Customer, Convert.ToDouble(Invoice_Total_Label.Text), Convert.ToInt32(Invoice_Pay_Label.Text), Convert.ToInt32(Invoice_Money_Give_Back_Label.Text));
            Sales_Form sale = new Sales_Form();
            sale.UserId = UserId;
            sale.BranchID = BranchID;
            sale.UserName = UserName;
            sale.Show();
            Close();
        }
        static void Print_PDF(int InvoiceID, List<Invoice_Print_Sale> invoiceItems, Customer_Class customer, double Total, int Money_Received, int Money_Return)
        {
            // Create a new document
            Document document = new Document();

            // Set the output file path
            string outputPath = "PDF File\\" + "Invoice#" + InvoiceID + ".pdf";

            FileStream combinedFs = new FileStream(outputPath, FileMode.Create);

            PdfWriter combinedWriter = PdfWriter.GetInstance(document, combinedFs);
            document.Open();
            Font font = FontFactory.GetFont("Century Gothic", 8f, Font.BOLD,
            iTextSharp.text.BaseColor.BLACK);//Initializing Information font

            string logoPath = Path.Combine(Environment.CurrentDirectory, "logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            document.Add(logo);

            // Add the logo to the document and Informations
            Paragraph paragraph = new Paragraph("Internation Book Centre\nEmail Address : IBCStore@Gmail.com\nPhone Number : 012 852 645\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_RIGHT;
            document.Add(paragraph);


            // Add Title
            Paragraph title = new Paragraph("INVOICE", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            // Add some space between the logo and the text
            document.Add(new Paragraph("\n\n"));

            paragraph = new Paragraph("Customer Detail : " + "\n", new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD));
            document.Add(paragraph);
            document.Add(new Paragraph("\n\n"));
            if (customer.Name != null) {
                paragraph = new Paragraph("Name : " + customer.Name + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
                document.Add(paragraph);
            }
            if (customer.Email != null)
            {
                paragraph = new Paragraph("Email : " + customer.Email + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
                document.Add(paragraph);
            }
            if (customer.PhoneNumber != null)
            {
                paragraph = new Paragraph("Phonenumber : " + customer.PhoneNumber + "\n", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
                document.Add(paragraph);
            }
            document.Add(new Paragraph("\n\n"));
            // Create a table with 7 columns
            var table = new PdfPTable(6);
            table.WidthPercentage = 100;

            table.DefaultCell.MinimumHeight = 20f;

            // Add the headers to the table
            table.AddCell(new Phrase("ID"));
            table.AddCell(new Phrase("Name"));
            table.AddCell(new Phrase("Quantity"));
            table.AddCell(new Phrase("Price"));
            table.AddCell(new Phrase("Total Price"));
            table.AddCell(CreateCenteredCell($"\r\n"));


            foreach (var invoice in invoiceItems)
            {
                table.AddCell(CreateCenteredCell($"{invoice.Id}"));
                table.AddCell(CreateCenteredCell(invoice.Name.ToString()));
                table.AddCell(CreateCenteredCell(invoice.Quantity.ToString()));
                table.AddCell(CreateCenteredCell($"{invoice.Price}"));
                table.AddCell(CreateCenteredCell($"{invoice.Total_Price}"));
                table.AddCell(CreateCenteredCell($"\r\n"));
            }

            paragraph = new Paragraph();
            paragraph.Add(table);
            document.Add(paragraph);

            paragraph = new Paragraph("\nTotal : " + Total, new Font(Font.FontFamily.HELVETICA, 10));
            paragraph.IndentationLeft = 390;
            document.Add(paragraph);

            paragraph = new Paragraph("\nMoney Received : " + Money_Received, new Font(Font.FontFamily.HELVETICA, 10));
            paragraph.IndentationLeft = 390;
            document.Add(paragraph);

            paragraph = new Paragraph("\nMoney Return : " + Money_Return, new Font(Font.FontFamily.HELVETICA, 10));
            paragraph.IndentationLeft = 390;
            document.Add(paragraph);
            // Close the document
            document.Close();
        }

        static private PdfPCell CreateCenteredCell(string content)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            return cell;
        }

        private void Exit_Invoice_Button_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
            Start_Printing_Grid.Visibility = Visibility.Hidden;
            Back_Grid_Item.Visibility = Visibility.Visible;
            Product_Detail_Show.Visibility = Visibility.Visible;
            Invoice_Detail_Show.Visibility = Visibility.Hidden;
        }

        private void Customer_Paid_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Customer_Paid.Text != "")
            {
                if (double.TryParse(Customer_Paid.Text, out double a))
                {

                }
                else
                {
                    Customer_Paid.Text = string.Empty;
                    MessageBox.Show("Incorrect format");
                }
            }
        }

        private void textbox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControl_Product_Show.Children.Clear();
            product_Output.Clear();
            string textbox = textbox_search.Text;
            List<Product_Class_Sale> products = null;
            if (int.TryParse(textbox, out int a))
            {
                products = Product_Search(textbox, BranchID, false);
            }
            else
            {
                products = Product_Search(textbox, BranchID);
            }
            if (products != null)
            {
                if (products.Count == 0)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < products.Count; i++)
                    {
                        Product_Show_UserControl product_UserControl = new Product_Show_UserControl();
                        product_UserControl.Product = products[i];
                        product_Output.Add(products[i]);
                        product_UserControl.Button_Click_Event += Product_UC_Button_Click_Event;
                        UserControl_Product_Show.Children.Add(product_UserControl);
                    }
                    Product_DataGrid.ItemsSource = product_Output;
                }
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Chat chat = new Chat(UserName);
            chat.Show();
        }
    }
}

