namespace NiboOfxParser.Models
{
    public class Statement
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public StatementStatus StatementStatus { get; set; }
        public StatementDatails StatementDatails { get; set; }
    }
}