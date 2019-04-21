using System.Data.Entity;

namespace NiboOfxParser.Models
{
    public class NiboOfxParserContext : DbContext
    {
        public NiboOfxParserContext() : base("name=NiboOfxParserContext")
        {
        }

        public DbSet<Statement> Statements { get; set; }
        public DbSet<StatementTransaction> StatementTransactions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<BankTransaction> BankTransactions { get; set; }
        public DbSet<StatementDatails> StatementDatails { get; set; }
        public DbSet<StatementStatus> StatementStatus { get; set; }

        public System.Data.Entity.DbSet<NiboOfxParser.Models.Bank> Banks { get; set; }
    }
}
