using DocumentFormat.OpenXml.Vml;
using IBC_Management_System.classdata;
using IBC_Management_System;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Quantity_Sale_Product.xaml
    /// </summary>
    public partial class Quantity_Sale_Product : Window
    {
        public List<Product_Class_Invoice> Product_In_Invoice { get; set; } = new List<Product_Class_Invoice>();
        public Customer_Class customer { get; set; }
        public Product_Class_Sale Product_Class_Sale { get; set; }
        public Product_Class_Invoice Delete_Product_Class { get; set; }
        public int UserID { get; set; }
        public int BranchID { get; set; }
        public string Username { get; set; }
        public string Texts { get; set; }
        public Quantity_Sale_Product()
        {
            InitializeComponent();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if ($"{Ok_Button.Content}".Contains("Add")) {

        //    }
        //    else
        //    {
        //    }
        //}
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Texts == "")
            {
                if (Product_Class_Sale.Picture != null)
                {
                    byte[] imageData = Product_Class_Sale.Picture;

                    BitmapImage bitmapImage = ConvertByteArrayToBitmapImage(imageData);

                    Product_Picture.Source = bitmapImage;
                }
                Add_Panel.Visibility = Visibility.Visible;
                Stock.Content = Product_Class_Sale.Stock;
                Ok_Button.HorizontalAlignment = HorizontalAlignment.Center;
                Remove_Panel.Visibility = Visibility.Hidden;
            }
            else
            {
                if (Delete_Product_Class.Picture != null)
                {
                    byte[] imageData = Delete_Product_Class.Picture;

                    BitmapImage bitmapImage = ConvertByteArrayToBitmapImage(imageData);

                    Product_Picture.Source = bitmapImage;
                }
                Add_Panel.Visibility = Visibility.Hidden;
                Remove_Panel.Visibility = Visibility.Visible;

                Stock.Content = Delete_Product_Class.Stock;
                Ok_Button.Content = Texts;
                Quantity_Textbox.Text = $"{Delete_Product_Class.Quantity}";
                Edit_Button.Visibility = Visibility.Visible;
            }
        }
        //private byte[] GetImageData()
        //{
        // Replace this with your actual image loading logic
        // For example, reading an image file into a byte array
        //string imagePath = "C:\\Users\\User\\Pictures\\Rany\\Couple pic.jfif";
        //   // byte[] imageData = File.ReadAllBytes(imagePath);
        //    return imageData;
        //}

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Sales_Form sales = new Sales_Form();
            sales.Product_In_Invoice = Product_In_Invoice;
            sales.customer = customer;
            sales.BranchID = BranchID;
            sales.UserId = UserID;
            sales.UserName = Username;
            sales.Show();
            Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Quantity_Textbox.Text != "")
            {
                bool IsHad = false;
                int.TryParse(Quantity_Textbox.Text, out int Inttemp);
                for (int i = 0; i < Product_In_Invoice.Count; i++)
                {
                    if (Product_In_Invoice[i].Id == Product_Class_Sale.Id)
                    {
                        if (Product_In_Invoice[i].Quantity + Inttemp <= Product_In_Invoice[i].Stock)
                        {
                            Product_In_Invoice[i].Quantity += Inttemp;
                            IsHad = true;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Quantity is higher than stock");
                            return;
                        }
                    }
                }
                if (IsHad == false)
                {
                    if (Inttemp <= Product_Class_Sale.Stock)
                    {
                        Product_Class_Invoice temp = new Product_Class_Invoice();
                        temp.Id = Product_Class_Sale.Id;
                        temp.Name = Product_Class_Sale.Name;
                        temp.Price = Product_Class_Sale.Price;
                        temp.Barcode = Product_Class_Sale.Barcode;
                        temp.Stock = Product_Class_Sale.Stock;
                        temp.Picture = Product_Class_Sale.Picture;
                        temp.Quantity = Inttemp;
                        Product_In_Invoice.Add(temp);
                    }
                    else
                    {
                        MessageBox.Show("Quantity is higher than stock");
                        return;
                    }
                }
                Sales_Form sales = new Sales_Form();
                sales.Product_In_Invoice = Product_In_Invoice;
                sales.customer = customer;
                sales.BranchID = BranchID;
                sales.UserId = UserID;
                sales.UserName = Username;
                sales.Show();
                Close();
            }
        }

        private void Quantity_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Quantity_Textbox.Text != "")
            {
                if (double.TryParse(Quantity_Textbox.Text, out double a))
                {
                    if (a >= 0)
                    {
                        if ($"{Ok_Button.Content}".Contains("Add"))
                        {
                            if ((double)Product_Class_Sale.Stock >= a)
                            {
                                Total.Content = $"{(double)Product_Class_Sale.Price * a}$";
                            }
                            else
                            {
                                MessageBox.Show("Quantity is higher than Stock");
                            }
                        }
                        else
                        {
                            if ((double)Delete_Product_Class.Stock >= a)
                            {
                                Total.Content = $"{(double)Delete_Product_Class.Price * a}$";
                            }
                            else
                            {
                                MessageBox.Show("Quantity is higher than Stock");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Quantity can not be lower than 0");
                        Quantity_Textbox.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Quantity can be only number");
                    Quantity_Textbox.Text = string.Empty;
                }
            }
            else
            {
                Total.Content = $"0";
            }
        }

        private void Edit_Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Quantity_Textbox.Text, out int Inttemp))
            {
                var temp = Delete_Product_Class;
                temp.Quantity = Inttemp;
                Product_In_Invoice.Remove(Delete_Product_Class);
                Product_In_Invoice.Add(temp);
                Sales_Form sales = new Sales_Form();
                sales.Product_In_Invoice = Product_In_Invoice;
                sales.customer = customer;
                sales.BranchID = BranchID;
                sales.UserName = Username;
                sales.UserId = UserID;
                sales.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Quantity incorrect format");
            }
        }
        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                memoryStream.Position = 0; // Reset the stream position
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Product_In_Invoice.Remove(Delete_Product_Class);
            Sales_Form sales = new Sales_Form();
            sales.Product_In_Invoice = Product_In_Invoice;
            sales.customer = customer;
            sales.BranchID = BranchID;
            sales.UserId = UserID;
            sales.UserName = Username;
            sales.Show();
            Close();
        }
    }
}
