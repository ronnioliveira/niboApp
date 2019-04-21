using System;

namespace NiboOfxParser.Models
{
    public class StatementTransaction
    {
        public string Type { get; set; }
        public DateTime PostedDate { get; set; }
        public double Value { get; set; }
        public long Id { get; set; }
        public string CustomId { get; set; }
        public int PayeeId { get; set; }
        public string Memo { get; set; }

        public StatementTransaction()
        {

        }
        public StatementTransaction(double value, string type, long id, DateTime postedDate, string memo, int payeeId)
        {
            var valueAux = Value.ToString().Replace('-', ' ').Replace('.', ' ').Trim();
            CustomId = $"{type}{id}{postedDate}{valueAux}{memo.Trim().Replace(" ","")}";

            this.Type = type;
            this.PostedDate = postedDate;
            this.Value = value;
            this.Id = id;
            this.PayeeId = payeeId;
            this.Memo = memo;
        }
    }
}