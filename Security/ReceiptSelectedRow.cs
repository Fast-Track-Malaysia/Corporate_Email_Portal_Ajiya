using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ReceiptSelectedRow
    {
        public int DocEntry { get; set; }
        public string SAPDocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        public bool isChecked { get; set; }
    }
}
