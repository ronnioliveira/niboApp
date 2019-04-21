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
    public class StatementStatusController : Controller
    {
        private NiboOfxParserContext db = new NiboOfxParserContext();

        // GET: StatementStatus
        public ActionResult Index()
        {
            return View(db.StatementStatus.ToList());
        }

        // GET: StatementStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatementStatus statementStatus = db.StatementStatus.Find(id);
            if (statementStatus == null)
            {
                return HttpNotFound();
            }
            return View(statementStatus);
        }

        // GET: StatementStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StatementStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Severity")] StatementStatus statementStatus)
        {
            if (ModelState.IsValid)
            {
                db.StatementStatus.Add(statementStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statementStatus);
        }

        // GET: StatementStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatementStatus statementStatus = db.StatementStatus.Find(id);
            if (statementStatus == null)
            {
                return HttpNotFound();
            }
            return View(statementStatus);
        }

        // POST: StatementStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Severity")] StatementStatus statementStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statementStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statementStatus);
        }

        // GET: StatementStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatementStatus statementStatus = db.StatementStatus.Find(id);
            if (statementStatus == null)
            {
                return HttpNotFound();
            }
            return View(statementStatus);
        }

        // POST: StatementStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StatementStatus statementStatus = db.StatementStatus.Find(id);
            db.StatementStatus.Remove(statementStatus);
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
