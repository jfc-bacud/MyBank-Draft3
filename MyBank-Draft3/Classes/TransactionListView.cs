using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBank_Draft3.Classes
{
    public class TransactionListView
    {
        public string Transact { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        //public string TransactionDesc { get; set; }
    }
}
