﻿using System;
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
    [Authorize(Roles = "Admin")]
    public class ClassesController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Classes
        public ActionResult Index()
        {
            return View(db.Classes.Where(e=>e.Active==1).OrderByDescending(e=>e.ID).ToList());
        }

        // GET: Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Class @class)
        {
            if (ModelState.IsValid)
            {   @class.Active = 1;
                db.Classes.Add(@class);
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index");
            }
            TempData["error"] = "asdasd";
            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Class @class)
        {
            if (ModelState.IsValid)
            {
                 @class.Active = 1;
                    db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index");
            }
            TempData["error"] = "asdasd";
            return View(@class);
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            @class.Active = 0;
            db.Entry(@class).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "asdasd";
            return RedirectToAction("Index");
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
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
