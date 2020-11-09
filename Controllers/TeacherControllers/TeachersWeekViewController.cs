using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class TeachersWeekViewController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: TeachersWeekView
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            var TeachersDates = db.TeachersDates.Where(e=>periodIDs.Contains(e.PeriodID)).Include(t => t.Class).Include(t => t.Cours).Include(t => t.User);
            ViewBag.sun = TeachersDates.Where(e=>e.Day=="1").Where(e=>e.TeacherID== id).OrderBy(e=>e.StratsAt);
            ViewBag.mon = TeachersDates.Where(e=>e.Day=="2").Where(e => e.TeacherID == id).OrderBy(e => e.StratsAt);
            ViewBag.tue = TeachersDates.Where(e=>e.Day=="3").Where(e => e.TeacherID == id).OrderBy(e => e.StratsAt);
            ViewBag.wen = TeachersDates.Where(e=>e.Day=="4").Where(e => e.TeacherID == id).OrderBy(e => e.StratsAt);
            ViewBag.thu = TeachersDates.Where(e=>e.Day=="5").Where(e => e.TeacherID == id).OrderBy(e => e.StratsAt);
            ViewBag.fri = TeachersDates.Where(e=>e.Day=="6").Where(e => e.TeacherID == id).OrderBy(e => e.StratsAt);
            ViewBag.sat = TeachersDates.Where(e=>e.Day=="7").Where(e => e.TeacherID == id).OrderBy(e => e.StratsAt);
           return View();
        }

        // GET: TeachersWeekView/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersDate teachersDate = db.TeachersDates.Find(id);
            if (teachersDate == null)
            {
                return HttpNotFound();
            }
            return View(teachersDate);
        }

        // GET: TeachersWeekView/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: TeachersWeekView/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClassID,TeacherID,CourseID,Day,StratsAt,FinishAT")] TeachersDate teachersDate)
        {
            if (ModelState.IsValid)
            {
                db.TeachersDates.Add(teachersDate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", teachersDate.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", teachersDate.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", teachersDate.TeacherID);
            return View(teachersDate);
        }

        // GET: TeachersWeekView/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersDate teachersDate = db.TeachersDates.Find(id);
            if (teachersDate == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", teachersDate.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", teachersDate.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", teachersDate.TeacherID);
            return View(teachersDate);
        }

        // POST: TeachersWeekView/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClassID,TeacherID,CourseID,Day,StratsAt,FinishAT")] TeachersDate teachersDate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teachersDate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", teachersDate.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", teachersDate.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", teachersDate.TeacherID);
            return View(teachersDate);
        }

        // GET: TeachersWeekView/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersDate teachersDate = db.TeachersDates.Find(id);
            if (teachersDate == null)
            {
                return HttpNotFound();
            }
            return View(teachersDate);
        }

        // POST: TeachersWeekView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeachersDate teachersDate = db.TeachersDates.Find(id);
            db.TeachersDates.Remove(teachersDate);
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
