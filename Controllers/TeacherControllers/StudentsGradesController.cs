using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;
using PagedList;

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class StudentsGradesController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StudentsGrades
        public ActionResult Index(int? QuizID, int? coursID, int? classID, int? page, string search)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.CourseID = coursID;
            ViewBag.classID = classID;
            ViewBag.quizID = QuizID;
            // the logged user ID = Teacher 
            int id = int.Parse(Session["userID"].ToString());


            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();



            var notes = db.Notes
                .Where(e=>e.QuizID==QuizID)
                .Where(e => e.User.Name.StartsWith(search) || search == null)
                .Include(n => n.Cours).Include(n => n.User).Include(n => n.Quiz);

           var QuizDate = db.Quizs.Where(e => e.ID == QuizID).Select(e => e.Date).First();
           var QuizHour = db.Quizs.Where(e => e.ID == QuizID).Select(e => e.EndHour).First();
            
            string QuizEndHour = DateTime.Parse(QuizHour).ToString("HH");
            string QuizEndMinute = DateTime.Parse(QuizHour).ToString("mm");
          
            DateTime QuizEendDate = DateTime.Parse(QuizDate.ToString());
            DateTime QuizEends = Functions.ChangeTime(QuizEendDate,int.Parse(QuizEndHour),int.Parse(QuizEndMinute),0,0);

             // the IDs of Student Sent Assignment 
            var SentstudentsID = notes.Select(e => e.StudentID).ToArray();
            if (DateTime.Now > QuizEends)
            {
               

                var StudenNotSentQuiz = db.StudentClasses.Where(e=>periodIDs.Contains(e.PeriodID))
             .Where(e => e.CoursID == coursID && e.ClassID == classID && e.TeacherID == id)
             .Where(p => !SentstudentsID.Contains(p.UserID))
                .ToList()
           ;
                foreach (var value in StudenNotSentQuiz)
                {
                    Note std = new Note();
                    std.StudentID = value.UserID;
                    std.CourseID = coursID.Value;
                    std.QuizID = QuizID.Value;
                     std.Note1 = "0";
                    db.Notes.Add(std);

                }
                db.SaveChanges();

            }



            return View(notes.ToList().ToPagedList(page ?? 1, 20));
        }






        public ActionResult CreateGrade([Bind(Include = "ID,Note1")] Note studentNotes)
        {

            Note studentnote = db.Notes.Find(studentNotes.ID);
            studentnote.Note1 = studentNotes.Note1;
            db.Entry(studentnote).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "StudentsGrades", new { QuizID = studentnote.QuizID, coursID = studentNotes.CourseID });

        }






        // GET: StudentsGrades/Details/5
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

        // GET: StudentsGrades/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name");
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title");
            return View();
        }

        // POST: StudentsGrades/Create
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

        // GET: StudentsGrades/Edit/5
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

        // POST: StudentsGrades/Edit/5
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

        // GET: StudentsGrades/Delete/5
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

        // POST: StudentsGrades/Delete/5
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
