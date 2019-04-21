using NiboOfxParser.CustomDataAnnotations;
using NiboOfxParser.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace NiboOfxParser.Controllers
{
    public class UploadController : Controller
    {
        private NiboOfxParserContext db = new NiboOfxParserContext();

        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var xmlStatement = GetStatement(file);
                }

                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        /// <summary>
        /// Main method to get the statement through OFX file
        /// </summary>
        /// <param name="ofxFile"></param>
        /// <returns>Coverted Statement</returns>
        private Statement GetStatement(HttpPostedFileBase ofxFile)
        {
            //Remove the file Header to use just the statement data
            var xmlString = TransformToXmlWithoutOfxHeader(ofxFile);

            //save xml nodes in your especific model
            SaveXmlNodes(xmlString);

            return new Statement();
        }

        /// <summary>
        /// Method to save the xml string file on database
        /// </summary>
        /// <param name="xmlString">XML as string</param>
        private void SaveXmlNodes(string xmlString)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            
            var bankTransaction = FillBankTransaction(xmlDoc); //Fill BankTransaction Object
            var account = FillAccount(xmlDoc); //Fill Account Object
            var balance = FillBalance(xmlDoc); //Fill Balance Object
            var statementDetails = FillStatementDetails(account, bankTransaction, balance, xmlDoc); //Fill StatementDetails Object
            var statementStatus = FillStatementStatus(xmlDoc); //Fill StatementStatus Object
            var statement = FillStatement(statementStatus, statementDetails, xmlDoc); //Fill Statement Object

            SaveStatement(statement);
        }

        /// <summary>
        /// Method to save entire statement
        /// </summary>
        /// <param name="statement">Statement object</param>
        [ValidateAntiForgeryToken]
        [StatementTransactionAlreadyExists]
        private void SaveStatement(Statement statement)
        {
            db.Statements.Add(statement);
            db.SaveChanges();
        }

        public void SaveStatementTransaction(StatementTransaction statementTransaction)
        {
            db.StatementTransactions.Add(statementTransaction);
            db.SaveChanges();
        }

        [ValidateAntiForgeryToken]
        private void SaveBankTransaction(BankTransaction bankTransaction)
        {
            db.BankTransactions.Add(bankTransaction);
            db.SaveChanges();
        }

        /// <summary>
        /// Method to fill Statement object
        /// </summary>
        /// <param name="statementStatusNodes">Statement XML nodes</param>
        /// <param name="statementStatus">StatementStatus object filled</param>
        /// <param name="bankTransaction">BankTransaction object filled</param>
        /// <returns>Statement object filled</returns>
        private Statement FillStatement(StatementStatus statementStatus, StatementDatails statementDatails, XmlDocument xmlDoc)
        {
            var xPath = "OFX/BANKMSGSRSV1/STMTTRNRS";
            var statementNodes = xmlDoc.SelectNodes(xPath);
            var statement = new Statement();

            foreach (XmlNode node in statementNodes)
            {
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "TRNUID":
                            statement.TransactionId = Convert.ToInt32(node.ChildNodes[i].InnerText);
                            break;
                    }
                }
            }

            statement.StatementStatus = statementStatus;
            statement.StatementDatails = statementDatails;

            return statement;
        }

        /// <summary>
        /// Method to fill StatementStatus object
        /// </summary>
        /// <param name="statementStatusNodes">StatementStatus XML node</param>
        /// <returns>StatementStatus object filled</returns>
        private StatementStatus FillStatementStatus(XmlDocument xmlDoc)
        {
            var xPath = "OFX/BANKMSGSRSV1/STMTTRNRS/STATUS";
            var statementStatusNodes = xmlDoc.SelectNodes(xPath);
            var statementStatus = new StatementStatus();

            foreach (XmlNode node in statementStatusNodes)
            {
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "CODE":
                            statementStatus.Code = node.ChildNodes[i].InnerText;
                            break;
                        case "SEVERITY":
                            statementStatus.Severity = node.ChildNodes[i].InnerText;
                            break;
                    }
                }
            }

            return statementStatus;
        }

        /// <summary>
        /// Method to fill StatementDetails object
        /// </summary>
        /// <param name="statementDetailsNodes">StatementDetails XML Nodes</param>
        /// <param name="account">Account object filled</param>
        /// <param name="bankTransaction">BankTransaction object filled</param>
        /// <returns>StatementDetails object filled</returns>
        private StatementDatails FillStatementDetails(Account account, BankTransaction bankTransaction, Balance balance, XmlDocument xmlDoc)
        {
            var xPath = "OFX/BANKMSGSRSV1/STMTTRNRS/STMTRS";
            var statementDetailsNodes = xmlDoc.SelectNodes(xPath);
            var statementDatails = new StatementDatails();

            foreach (XmlNode node in statementDetailsNodes)
            {
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "CURDEF":
                            statementDatails.Currency = node.ChildNodes[i].InnerText;
                            break;
                    }
                }
            }

            statementDatails.Account = account;
            statementDatails.BankTransaction = bankTransaction;
            statementDatails.Balance = balance;

            return statementDatails;
        }

        /// <summary>
        /// Method to fill Balance object
        /// </summary>
        /// <param name="balanceNodes"></param>
        /// <returns>Balance object filled</returns>
        private Balance FillBalance(XmlDocument xmlDoc)
        {
            var xPath = "OFX/BANKMSGSRSV1/STMTTRNRS/STMTRS/LEDGERBAL";
            var balanceNodes = xmlDoc.SelectNodes(xPath);
            var balance = new Balance();

            foreach (XmlNode node in balanceNodes)
            {
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "BALAMT":
                            balance.Value = Convert.ToDouble(node.ChildNodes[i].InnerText, CultureInfo.GetCultureInfo("en-US"));
                            break;
                        case "DTASOF":
                            balance.Date = ExtractDate(node.ChildNodes[i].InnerText);
                            break;
                    }
                }
            }

            return balance;
        }

        /// <summary>
        /// Method to fill Account object
        /// </summary>
        /// <param name="accountNodes">Account XML node</param>
        /// <returns>Account object filled</returns>
        private Account FillAccount(XmlDocument xmlDoc)
        {
            var xPath = "OFX/BANKMSGSRSV1/STMTTRNRS/STMTRS/BANKACCTFROM";
            var accountNodes = xmlDoc.SelectNodes(xPath);

            var account = new Account();

            foreach (XmlNode node in accountNodes)
            {
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "BANKID":
                            account.BankId = Convert.ToInt32(node.ChildNodes[i].InnerText);
                            break;
                        case "ACCTID":
                            account.Id = Convert.ToInt32(node.ChildNodes[i].InnerText);
                            break;
                        case "ACCTTYPE":
                            account.Type = node.ChildNodes[i].InnerText;
                            break;                        
                    }
                }
            }

            return account;
        }

        /// <summary>
        /// Method to fill the BankTransaction object
        /// </summary>
        /// <param name="nodes">Bank Transaction XML Node</param>
        /// <returns>Bank Transaction object filled</returns>
        private BankTransaction FillBankTransaction(XmlDocument xmlDoc)
        {
            var xPath = "OFX/BANKMSGSRSV1/STMTTRNRS/STMTRS/BANKTRANLIST";
            var nodes = xmlDoc.SelectNodes(xPath);

            var statementTransactionList = new List<StatementTransaction>();
            var bankTransaction = new BankTransaction();

            //loop into nodes
            foreach (XmlNode node in nodes)
            {
                //loop into nodes
                for(var i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "DTSTART")
                        bankTransaction.StartDate = ExtractDate(node.ChildNodes[i].InnerText);

                    if (node.ChildNodes[i].Name == "DTEND")
                        bankTransaction.EndDate = ExtractDate(node.ChildNodes[i].InnerText);

                    //verify if is statement transaction node
                    if (node.ChildNodes[i].Name == "STMTTRN")
                    {
                        //fill statementTransaction object
                        var validStatementTransaction = FillStatementTransactions(node.ChildNodes[i]);

                        //Verify if object is valid to insert on the list
                        if(validStatementTransaction != null)
                            statementTransactionList.Add(validStatementTransaction);//add each statement transaction item
                    }
                }
            }

            bankTransaction.StatementTransactionList = statementTransactionList;
            return bankTransaction;
        }

        /// <summary>
        /// Method to fill Statement Transaction Object
        /// </summary>
        /// <param name="node">Statement transaction XML Node</param>
        /// <returns>Statement transaction Filled</returns>
        public StatementTransaction FillStatementTransactions(XmlNode node)
        {
            var statementTransaction = new StatementTransaction();
            var statementTransactionList = new List<StatementTransaction>();

            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                switch (node.ChildNodes[i].Name)
                {
                    case "TRNTYPE":
                        statementTransaction.Type = node.ChildNodes[i].InnerText;
                        break;
                    case "DTPOSTED":
                        statementTransaction.PostedDate = ExtractDate(node.ChildNodes[i].InnerText);
                        break;
                    case "TRNAMT":
                        statementTransaction.Value = Convert.ToDouble(node.ChildNodes[i].InnerText, CultureInfo.GetCultureInfo("en-US"));
                        break;
                    case "FITID":
                        statementTransaction.Id = Convert.ToInt64(node.ChildNodes[i].InnerText);
                        break;
                    case "MEMO":
                        statementTransaction.Memo = node.ChildNodes[i].InnerText;
                        break;
                    case "PAYEEID":
                        statementTransaction.PayeeId = Convert.ToInt32(node.ChildNodes[i].InnerText);
                        break;
                }
            }

            //creato object with customId
            var statementTransactionWithCustomId = new StatementTransaction(statementTransaction.Value,
                                                                            statementTransaction.Type,
                                                                            statementTransaction.Id,
                                                                            statementTransaction.PostedDate,
                                                                            statementTransaction.Memo,
                                                                            statementTransaction.PayeeId);

            //Vefify if customId already existis on database
            var statementTransactionAux = GetStatementTransactionByCustomId(statementTransactionWithCustomId.CustomId);

            //if customId exists, return null
            if (statementTransactionAux != null)
                return null;

            //if customId not exists, return object filled to save
            return statementTransactionWithCustomId;
        }

        private StatementTransaction GetStatementTransactionByCustomId(string customId)
        {
            return db.StatementTransactions.FirstOrDefault(x => x.CustomId == customId);
        }

        /// <summary>
        /// Remove useless ofx header and tranform in xm.
        /// </summary>
        /// <param name="ofxFile"></param>
        /// <returns>file without header</returns>
        private string TransformToXmlWithoutOfxHeader(HttpPostedFileBase ofxFile)
        {
            var fileWithoutHeader = new StringBuilder();
            string line;
            var continueWritting = true;

            //var streamReader = System.IO.File.OpenText(ofxFile);
            var streamReader = new StreamReader(ofxFile.InputStream);

            while ((line = streamReader.ReadLine()) != null && continueWritting)
            {
                line = line.Trim();

                if (line.StartsWith("<BANKMSGSRSV1>"))
                {
                    //insert xml header
                    var xmlHeader = @"<?xml version=""1.0""?>";
                    fileWithoutHeader.AppendLine(xmlHeader);
                    
                    //write first tag
                    fileWithoutHeader.AppendLine("<OFX>");
                    fileWithoutHeader.AppendLine(line);

                    while((line = streamReader.ReadLine()) != null)
                    {
                        line = InsertEndTag(line.Trim());

                        //write the rest of lines without header
                        fileWithoutHeader.AppendLine(line);
                    }
                    //finish the new file
                    continueWritting = false;
                }
            }
            return fileWithoutHeader.ToString();
        }

        /// <summary>
        /// Insert endtag to single tags
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string InsertEndTag(string line)
        {
            //is closed tag
            if (line.StartsWith("</"))
                return line;

            //initialize tagName
            var tagName = string.Empty;
            
            //get the tag name
            if (line.StartsWith("<") && line.EndsWith(">"))
                tagName = line.Substring(1, line.Length - 2);

            //get the tag name
            if (line.StartsWith("<") && !line.EndsWith(">"))
                tagName = line.Split('>')[0].Substring(1);

            if (line.StartsWith("<") && !line.StartsWith("</") && GetUnclosedTag(tagName))
                return $"{line}</{tagName}>"; //insert end tag
            else
                return line;
        }

        /// <summary>
        /// Method to verify if tag is unclosed
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns>if tag is unclosed</returns>
        private bool GetUnclosedTag(string tagName)
        {
            //closed tags array
            var closedTags = new string[] {
                "OFX",
                "BANKMSGSRSV1",
                "STMTTRNRS",
                "STATUS",
                "STMTRS",
                "BANKACCTFROM",
                "BANKTRANLIST",
                "STMTTRN",
                "LEDGERBAL"
            };

            //verify if tag name is on array
            if(Array.FindAll(closedTags, x => x.Equals(tagName)).Any())
                return false;

            return true;
        }

        /// <summary>
        /// Method to convert a string date to a DateTime object
        /// </summary>
        /// <param name="date"></param>
        /// <returns>DateTime object</returns>
        private DateTime ExtractDate(string date)
        {
            var year = Convert.ToInt32(date.Substring(0, 4));
            var month = Convert.ToInt32(date.Substring(4, 2));
            var day = Convert.ToInt32(date.Substring(6, 2));

            return new DateTime(year, month, day);
        }
    }
}