using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NiboOfxParser.Models;

namespace NiboOfxParser.Controllers
{
    public class StatementTransactionsController : Controller
    {
        private NiboOfxParserContext db = new NiboOfxParserContext();

        // GET: StatementTransactions
        public ActionResult Index()
        {
            return View(db.StatementTransactions.ToList());
        }

        // GET: StatementTransactions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatementTransaction statementTransaction = db.StatementTransactions.Find(id);
            if (statementTransaction == null)
            {
                return HttpNotFound();
            }
            return View(statementTransaction);
        }

        // GET: StatementTransactions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StatementTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,PostedDate,Value,CustomId,PayeeId,Memo")] StatementTransaction statementTransaction)
        {
            if (ModelState.IsValid)
            {
                db.StatementTransactions.Add(statementTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statementTransaction);
        }

        // GET: StatementTransactions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatementTransaction statementTransaction = db.StatementTransactions.Find(id);
            if (statementTransaction == null)
            {
                return HttpNotFound();
            }
            return View(statementTransaction);
        }

        // POST: StatementTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,PostedDate,Value,CustomId,PayeeId,Memo")] StatementTransaction statementTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statementTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statementTransaction);
        }

        // GET: StatementTransactions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatementTransaction statementTransaction = db.StatementTransactions.Find(id);
            if (statementTransaction == null)
            {
                return HttpNotFound();
            }
            return View(statementTransaction);
        }

        // POST: StatementTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            StatementTransaction statementTransaction = db.StatementTransactions.Find(id);
            db.StatementTransactions.Remove(statementTransaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
