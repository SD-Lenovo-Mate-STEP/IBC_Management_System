using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBC_Management_System.classdata
{
    public class Product_Class_Sale
    {
        public Product_Class_Sale()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public byte[] Picture { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
    }
    public class Product_Class_Invoice
    {
        public Product_Class_Invoice()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public byte[] Picture { get; set; }
        public string Barcode { get; set; }
        public int Quantity { get; set; }
    }
}
