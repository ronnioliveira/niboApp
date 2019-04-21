using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiboOfxParser.Models;

namespace NiboOfxParser.Controllers
{
    public class StatementsController : Controller
    {
        private NiboOfxParserContext db = new NiboOfxParserContext();

        // GET: Statements
        public ActionResult Index()
        {
            //var completeStatement = new CompleteStatement();
            //var statementList = db.Statements.ToList();


            //var stTransaction = db.StatementTransactions.ToList();

            //foreach (var item in stTransaction)
            //{
            //    item.
            //}



            //    foreach (var item in statementList)
            //{
            //    item.StatementDatails = db.StatementDatails.Find(item.StatementDatails.Id);
            //}

            return View(db.Statements.ToList());
        }

        // GET: Statements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Statement statement = db.Statements.Find(id);
            if (statement == null)
            {
                return HttpNotFound();
            }
            return View(statement);
        }

        // GET: Statements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Statements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TransactionId")] Statement statement)
        {
            if (ModelState.IsValid)
            {
                db.Statements.Add(statement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statement);
        }

        // GET: Statements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Statement statement = db.Statements.Find(id);
            if (statement == null)
            {
                return HttpNotFound();
            }
            return View(statement);
        }

        // POST: Statements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TransactionId")] Statement statement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statement);
        }

        // GET: Statements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Statement statement = db.Statements.Find(id);
            if (statement == null)
            {
                return HttpNotFound();
            }
            return View(statement);
        }

        // POST: Statements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Statement statement = db.Statements.Find(id);
            db.Statements.Remove(statement);
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
