using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBC_Management_System.classdata
{
    public class Sales_Class_Sale
    {
        public Sales_Class_Sale()
        {

        }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Product_ID { get; set; }
        public string Invoice_ID { get; set; }
    }
}
