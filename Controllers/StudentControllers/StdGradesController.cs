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
    public class StdGradesController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdGrades
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            var studentClasses = db.StudentClasses.Where(e => periodIDs.Contains(e.PeriodID)).Where(e => e.UserID == id);
          
            return View(studentClasses.ToList());
        }



        public ActionResult QuizsesGrades(int? CourseID, int? ClassID, int? TeacherID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }


            int id = int.Parse(Session["userID"].ToString());

            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            var stdquizs = db.Quizs.Where(e => e.TeacherID == TeacherID && e.CourseID == CourseID && e.ClassID == ClassID);
            var quizIds = stdquizs.Select(e => e.ID).ToArray();
            var STDQuizsesGrades = db.Notes.Where(e => e.StudentID == id  ).Where(e=> quizIds.Contains(e.QuizID));

            return PartialView(STDQuizsesGrades.ToList());
        }
        public ActionResult AssiginmentGrades(int? CourseID, int? ClassID, int? TeacherID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int id = int.Parse(Session["userID"].ToString());
            var STDAssiginmentGrades = db.StudentAssignments.Where(e => e.StudentID == id && e.TeacherID == TeacherID && e.CourseID == CourseID && e.ClassID == ClassID);

            return PartialView(STDAssiginmentGrades.ToList());
        }




        // GET: StdGrades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: StdGrades/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name");
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title");
            return View();
        }

        // POST: StdGrades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,QuizID,StudentID,Note1,CourseID")] Note note)
        {
            if (ModelState.IsValid)
            {
                db.Notes.Add(note);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", note.CourseID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", note.StudentID);
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", note.QuizID);
            return View(note);
        }

        // GET: StdGrades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", note.CourseID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", note.StudentID);
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", note.QuizID);
            return View(note);
        }

        // POST: StdGrades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,QuizID,StudentID,Note1,CourseID")] Note note)
        {
            if (ModelState.IsValid)
            {
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", note.CourseID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", note.StudentID);
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", note.QuizID);
            return View(note);
        }

        // GET: StdGrades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: StdGrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
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
