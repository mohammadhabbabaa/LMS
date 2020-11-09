using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;

namespace Kurs.Controllers.StudentControllers
{
    public class StdClassesController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdClasses
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            var studentClasses = db.StudentClasses.Where(e => periodIDs.Contains(e.PeriodID)).Where(e=>e.UserID== id).Include(s => s.Class).Include(s => s.Cours).Include(s => s.User).Include(s => s.User1).Include(s => s.Period);
            return View(studentClasses.ToList());
        }

        // GET: StdClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentClass studentClass = db.StudentClasses.Find(id);
            if (studentClass == null)
            {
                return HttpNotFound();
            }
            return View(studentClass);
        }

        // GET: StdClasses/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name");
            return View();
        }

        // POST: StdClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,ClassID,TeacherID,CoursID,PeriodID")] StudentClass studentClass)
        {
            if (ModelState.IsValid)
            {
                db.StudentClasses.Add(studentClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", studentClass.ClassID);
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name", studentClass.CoursID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", studentClass.UserID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentClass.TeacherID);
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", studentClass.PeriodID);
            return View(studentClass);
        }

        // GET: StdClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentClass studentClass = db.StudentClasses.Find(id);
            if (studentClass == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", studentClass.ClassID);
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name", studentClass.CoursID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", studentClass.UserID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentClass.TeacherID);
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", studentClass.PeriodID);
            return View(studentClass);
        }

        // POST: StdClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,ClassID,TeacherID,CoursID,PeriodID")] StudentClass studentClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", studentClass.ClassID);
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name", studentClass.CoursID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", studentClass.UserID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentClass.TeacherID);
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", studentClass.PeriodID);
            return View(studentClass);
        }

        // GET: StdClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentClass studentClass = db.StudentClasses.Find(id);
            if (studentClass == null)
            {
                return HttpNotFound();
            }
            return View(studentClass);
        }

        // POST: StdClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentClass studentClass = db.StudentClasses.Find(id);
            db.StudentClasses.Remove(studentClass);
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
