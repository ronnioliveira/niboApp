using System;
using System.Collections.Generic;

namespace NiboOfxParser.Models
{
    public class BankTransaction
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<StatementTransaction> StatementTransactionList { get; set; }
    }
}