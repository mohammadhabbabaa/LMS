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
    [Authorize(Roles = "Admin")]
    public class StudentClassesController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StudentClasses
        public ActionResult Index(int? id)
        {
                 ViewBag.UserID = id;
                 var studentClasses = db.StudentClasses
                 .Where(e => e.UserID == id)
                .Include(s => s.Class).Include(s => s.User).Include(s => s.User1).Include(s => s.Cours).OrderByDescending(e=>e.ID);
            return View(studentClasses.ToList());
        }

        // GET: StudentClasses/Details/5
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

        // GET: StudentClasses/Create
        public ActionResult Create(int? id)
        {
            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name");
             ViewBag.CoursID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name");
             ViewBag.PeriodID = new SelectList(db.Periods.Where(e => e.EndDate >= DateTime.Now).OrderByDescending(e=>e.ID), "ID", "Name");
             ViewBag.StudentID = id;
            ViewBag.TeacherID = new SelectList(db.Users.Where(e => e.Active == 1 && e.UserTaype==2), "ID", "Name");
            return View();
        }

        // POST: StudentClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassID,TeacherID,CoursID,PeriodID")] StudentClass studentClass, int id)
        {
            if (ModelState.IsValid)
            {
                studentClass.UserID = id;
                db.StudentClasses.Add(studentClass);
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index", "StudentClasses", new { id = id });
            }

            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name", studentClass.ClassID);
            ViewBag.CoursID = new SelectList(db.Users.Where(e => e.Active == 1), "ID", "Name", studentClass.CoursID);
            ViewBag.TeacherID = new SelectList(db.Users.Where(e => e.Active == 1 && e.UserTaype==2 ), "ID", "Name", studentClass.TeacherID);
            return View(studentClass);
        }

        // GET: StudentClasses/Edit/5
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
            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name", studentClass.ClassID);
 
            ViewBag.CoursID = new SelectList(db.Users.Where(e => e.Active == 1), "ID", "Name", studentClass.CoursID);
            ViewBag.TeacherID = new SelectList(db.Users.Where(e => e.Active == 1 && e.UserTaype == 2), "ID", "Name", studentClass.TeacherID);

            return View(studentClass);
        }

        // POST: StudentClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassID,TeacherID,CoursID")] StudentClass studentClass,int id)
        {
            if (ModelState.IsValid)
            {
                StudentClass mystd = db.StudentClasses.Find(id);
                mystd.TeacherID = studentClass.TeacherID;
                mystd.CoursID = studentClass.CoursID;
                mystd.ClassID = studentClass.CoursID;
                db.Entry(mystd).Property(u => u.CoursID).IsModified = false;
                db.Entry(mystd).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index", "StudentClasses", new { id = mystd.UserID });
            }
            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name", studentClass.ClassID);
            ViewBag.CoursID = new SelectList(db.Users.Where(e => e.Active == 1), "ID", "Name", studentClass.CoursID);
            ViewBag.TeacherID = new SelectList(db.Users.Where(e => e.Active == 1 && e.UserTaype == 2), "ID", "Name", studentClass.TeacherID);

            return View(studentClass);
        }

        // GET: StudentClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            StudentClass studentClass = db.StudentClasses.Find(id);
            int userid = studentClass.UserID.Value;
            db.StudentClasses.Remove(studentClass);
            db.SaveChanges();
            TempData["success"] = "asdasd";
            return RedirectToAction("Index", "StudentClasses", new { id = userid });
        }

        // POST: StudentClasses/Delete/5
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
