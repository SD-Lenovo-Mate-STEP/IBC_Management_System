using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBC_Management_System.classdata
{
    public class Invoice_Class_Sale
    {
        public Invoice_Class_Sale()
        {

        }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public int Userid { get; set; }
        public int Branchid { get; set; }
        public int Customerid { get; set; }
    }
    public class Invoice_Print_Sale
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total_Price { get; set; }
    }
}
