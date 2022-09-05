using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class LInvoiceSelectedRow
    {
        public int ID { get; set; }
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public int PortalOid { get; set; }
        public string PortalDocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime SendDate { get; set; }
        public bool isChecked { get; set; }
    }
}
