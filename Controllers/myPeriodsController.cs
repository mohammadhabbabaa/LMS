using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;

namespace Kurs.Controllers
{
    public class myPeriodsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: myPeriods
        public ActionResult Index()
        {
          
            return View(db.Periods.Where(e => e.EndDate >= DateTime.Now).ToList());
        }

        // GET: myPeriods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = db.Periods.Find(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // GET: myPeriods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: myPeriods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,EndDate")] Period period)
        {
            if (ModelState.IsValid)
            {
                db.Periods.Add(period);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(period);
        }

        // GET: myPeriods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = db.Periods.Find(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // POST: myPeriods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,EndDate")] Period period)
        {
            if (ModelState.IsValid)
            {
                db.Entry(period).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(period);
        }

        // GET: myPeriods/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = db.Periods.Find(id);
            db.Periods.Remove(period);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: myPeriods/Delete/5
    

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
