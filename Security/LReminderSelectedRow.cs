using System;

namespace Security
{
    public class LReminderSelectedRow
    {
        public int ID { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public DateTime ReminderDate { get; set; }
        public decimal MinAmt { get; set; }
        public int InvDaysFrom { get; set; }
        public int InvDaysTo { get; set; }
        public string LetterType { get; set; }
        public DateTime SendDate { get; set; }
        public bool isChecked { get; set; }
    }
}
