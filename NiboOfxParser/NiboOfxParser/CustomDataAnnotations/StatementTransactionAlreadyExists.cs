using NiboOfxParser.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NiboOfxParser.CustomDataAnnotations
{
    public class StatementTransactionAlreadyExists : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var statementTransaction = validationContext.ObjectInstance as StatementTransaction;
            if (statementTransaction == null) return new ValidationResult("Statement Transaction is empty");

            NiboOfxParserContext niboContext = new NiboOfxParserContext();

            var customId = new StatementTransaction(
                statementTransaction.Value,
                statementTransaction.Type,
                statementTransaction.Id,
                statementTransaction.PostedDate,
                statementTransaction.Memo,
                statementTransaction.PayeeId).CustomId;

            var customIdAlreadyExists = niboContext.StatementTransactions.FirstOrDefault(x => x.CustomId == customId);

            if (customIdAlreadyExists == null)
                return ValidationResult.Success;
            else
                return new ValidationResult($"The statement transaction already exists");
            // return base.IsValid(value, validationContext);
        }
    }
}