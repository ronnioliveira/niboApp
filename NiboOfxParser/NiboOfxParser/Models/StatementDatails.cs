
namespace NiboOfxParser.Models
{
    public class StatementDatails
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public Account Account { get; set; }
        public BankTransaction BankTransaction { get; set; }
        public Balance Balance { get; set; }
    }
}