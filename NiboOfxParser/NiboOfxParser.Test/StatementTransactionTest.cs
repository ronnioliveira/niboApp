using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiboOfxParser.Controllers;

namespace NiboOfxParser.Test
{
    [TestClass]
    public class StatementTransactionTest
    {
        [TestMethod]
        public void SaveNewTransaction_IfExists_ShouldNotSave()
        {
            //arrange
            var xmlFile1 = @"";
            var xmlFile2 = @"";


            //act
            var uploadController = new UploadController();
            var statementTransaction = uploadController.FillStatementTransactions()

            var stTransactionController = new StatementTransactionsController();

            stTransactionController.Create()
               

            //assert

        }

        [TestMethod]
        public void SaveNewTransaction_IfNotExists_Save()
        {
        }
    }
}
