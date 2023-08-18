using IBC_Management_System.classdata;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Product_Show_UserControl.xaml
    /// </summary>
    public partial class Product_Show_UserControl : UserControl
    {
        public Product_Class_Sale Product { get; set; }

        public class Button_Click_Invoke : EventArgs
        {
            public Product_Class_Sale Product { get; set; }
        }

        public event EventHandler<Button_Click_Invoke> Button_Click_Event;
        public Product_Show_UserControl()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var handler = Button_Click_Event;
            if (handler != null)
            {
                handler(this, new Button_Click_Invoke
                { Product = Product });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Name_Product.Content = "Name : " + Product.Name;
            Stock.Content = "Stock : " + Product.Stock;
            Barcode.Content = "Barcode : " + Product.Barcode;
            Price.Content = "Price : " + Product.Price;
            if (Product.Picture != null)
            {
                byte[] imageData = Product.Picture;

                BitmapImage bitmapImage = ConvertByteArrayToBitmapImage(imageData);

                Picture.Source = bitmapImage;
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
                //bitmapImage.PixelHeight = 
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}
