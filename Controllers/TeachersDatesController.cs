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
    public class TeachersDatesController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: TeachersDates
        public ActionResult Index(int ? id)
        {
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            var teachersDates = db.TeachersDates
                .Where(e=>e.TeacherID==id).Where(e => periodIDs.Contains(e.PeriodID))
                .Include(t => t.Class).Include(t => t.Cours).Include(t => t.User).OrderBy(e=>e.Day).ThenBy(x => x.StratsAt);
            return View(teachersDates.ToList());
        }

        // GET: TeachersDates/Details/5
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

        // GET: TeachersDates/Create
        public ActionResult Create(int id)
        {
            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users.Where(e => e.UserTaype == 2 && e.Active == 1), "ID", "Name");
            ViewBag.PeriodID = new SelectList(db.Periods.Where(e => e.EndDate >= DateTime.Now).OrderByDescending(e => e.ID), "ID", "Name");

            return View();
        }

        // POST: TeachersDates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassID,CourseID,Day,StratsAt,FinishAT,PeriodID")] TeachersDate teachersDate,int id)
        {
            if (ModelState.IsValid)
            {
                teachersDate.TeacherID = id;
                db.TeachersDates.Add(teachersDate);
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Create", "TeachersDates", new { id  });
            }

            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name", teachersDate.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name", teachersDate.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users.Where(e => e.UserTaype == 2 && e.Active == 1), "ID", "Name", teachersDate.TeacherID);
            TempData["error"] = "asdasd";
            return View(teachersDate);
        }

        // GET: TeachersDates/Edit/5
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
            ViewBag.ClassID = new SelectList(db.Classes.Where(e => e.Active == 1), "ID", "Name", teachersDate.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name", teachersDate.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users.Where(e=>e.UserTaype==2 && e.Active == 1), "ID", "Name", teachersDate.TeacherID);


            return View(teachersDate);
        }

        // POST: TeachersDates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassID,CourseID,Day,StratsAt,FinishAT")] TeachersDate teachersDate,int id)
        {
            if (ModelState.IsValid)
            {
                TeachersDate myteachersDate = db.TeachersDates.Find(id);
                myteachersDate.ClassID = teachersDate.ClassID;
                myteachersDate.CourseID = teachersDate.CourseID;
                myteachersDate.Day = teachersDate.Day;
                myteachersDate.StratsAt = teachersDate.StratsAt;
                myteachersDate.FinishAT = teachersDate.FinishAT;

                db.Entry(myteachersDate).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index", "TeachersDates", new {id= myteachersDate.TeacherID });
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", teachersDate.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", teachersDate.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", teachersDate.TeacherID);
            TempData["error"] = "asdasd";
            return View(teachersDate);
        }

        // GET: TeachersDates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersDate teachersDate = db.TeachersDates.Find(id);
            int tid = teachersDate.TeacherID.Value;

            db.TeachersDates.Remove(teachersDate);
            db.SaveChanges();
            TempData["success"] = "asdasd";
            return RedirectToAction("Index", "TeachersDates", new { id = tid });
        }

        // POST: TeachersDates/Delete/5
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
