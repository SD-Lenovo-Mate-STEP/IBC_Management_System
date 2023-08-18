using System;
using System.Windows;

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Category_Button_User.xaml
    /// </summary>
    public partial class Category_Button_User : System.Windows.Controls.UserControl
    {
        public Category_Class_Sale Category_Class_Sale { get; set; }

        public class Button_Click_Invoke : EventArgs
        {
            public Category_Class_Sale Category_Class_Sale { get; set; }
        }

        public event EventHandler<Button_Click_Invoke> Button_Click_Event;
        public Category_Button_User()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var handler = Button_Click_Event;
            if (handler != null)
            {
                handler(this, new Button_Click_Invoke
                { Category_Class_Sale = Category_Class_Sale });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Category_Text.Width = this.Width;
            Category_Text.Height = this.Height;
            Category_Text.Content = Category_Class_Sale.Name;
        }
    }
}
