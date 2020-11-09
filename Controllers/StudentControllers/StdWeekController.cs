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
    public class StdWeekController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdWeek
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int id = int.Parse(Session["userID"].ToString());
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e=>e.ID).ToArray();

            var stdentClasses = db.StudentClasses.Where(e=> periodIDs.Contains(e.PeriodID)).Where(e => e.UserID == id);
            var stdclass = stdentClasses.Select(e => e.ClassID).ToArray();
            var stdcourse = stdentClasses.Select(e => e.CoursID).ToArray();
            var stdTeachers = stdentClasses.Select(e => e.TeacherID).ToArray();

            var TeachersDates = db.TeachersDates.Include(t => t.Class).Include(t => t.Cours).Include(t => t.User);
            ViewBag.sun = TeachersDates.Where(e => e.Day == "1").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID)&& stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            ViewBag.mon = TeachersDates.Where(e => e.Day == "2").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID) && stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            ViewBag.tue = TeachersDates.Where(e => e.Day == "3").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID) && stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            ViewBag.wen = TeachersDates.Where(e => e.Day == "4").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID) && stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            ViewBag.thu = TeachersDates.Where(e => e.Day == "5").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID) && stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            ViewBag.fri = TeachersDates.Include(t => t.User).Where(e => e.Day == "6").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID) && stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            ViewBag.sat = TeachersDates.Include(t => t.User).Where(e => e.Day == "7").Where(e => stdclass.Contains(e.ClassID) && stdcourse.Contains(e.CourseID) && stdTeachers.Contains(e.TeacherID)).OrderBy(e => e.StratsAt);
            return View();
        }

        // GET: StdWeek/Details/5
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

        // GET: StdWeek/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name");
            return View();
        }

        // POST: StdWeek/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClassID,TeacherID,CourseID,Day,StratsAt,FinishAT,PeriodID")] TeachersDate teachersDate)
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
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", teachersDate.PeriodID);
            return View(teachersDate);
        }

        // GET: StdWeek/Edit/5
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
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", teachersDate.PeriodID);
            return View(teachersDate);
        }

        // POST: StdWeek/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClassID,TeacherID,CourseID,Day,StratsAt,FinishAT,PeriodID")] TeachersDate teachersDate)
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
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", teachersDate.PeriodID);
            return View(teachersDate);
        }

        // GET: StdWeek/Delete/5
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

        // POST: StdWeek/Delete/5
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
