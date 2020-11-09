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
    public class StudentQuizAnswaresController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StudentQuizAnswares
        public ActionResult Index(int? QuizID,int? stdID)
        {
            var studentQuizs = db.StudentQuizs
                
                .Where(e=>e.QuizID==QuizID && e.StudentID==stdID)
                .Include(s => s.Question).Include(s => s.Quiz).Include(s => s.User);
            var myQuiz = db.Quizs.Where(e=>e.ID==QuizID).FirstOrDefault();
            ViewBag.CoursID = myQuiz.CourseID;
            ViewBag.ClassID = myQuiz.ClassID;
            ViewBag.QuizID = myQuiz.ID;

            return View(studentQuizs.ToList());
        }

        // GET: StudentQuizAnswares/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentQuiz studentQuiz = db.StudentQuizs.Find(id);
            if (studentQuiz == null)
            {
                return HttpNotFound();
            }
            return View(studentQuiz);
        }

        // GET: StudentQuizAnswares/Create
        public ActionResult Create()
        {
            ViewBag.QustionID = new SelectList(db.Questions, "ID", "Puan");
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title");
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: StudentQuizAnswares/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StudentID,QuizID,QustionID,Answer,Puan,RightAnswer")] StudentQuiz studentQuiz)
        {
            if (ModelState.IsValid)
            {
                db.StudentQuizs.Add(studentQuiz);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QustionID = new SelectList(db.Questions, "ID", "Puan", studentQuiz.QustionID);
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", studentQuiz.QuizID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", studentQuiz.StudentID);
            return View(studentQuiz);
        }

        // GET: StudentQuizAnswares/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentQuiz studentQuiz = db.StudentQuizs.Find(id);
            if (studentQuiz == null)
            {
                return HttpNotFound();
            }
            ViewBag.QustionID = new SelectList(db.Questions, "ID", "Puan", studentQuiz.QustionID);
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", studentQuiz.QuizID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", studentQuiz.StudentID);
            return View(studentQuiz);
        }

        // POST: StudentQuizAnswares/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StudentID,QuizID,QustionID,Answer,Puan,RightAnswer")] StudentQuiz studentQuiz)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentQuiz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QustionID = new SelectList(db.Questions, "ID", "Puan", studentQuiz.QustionID);
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", studentQuiz.QuizID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", studentQuiz.StudentID);
            return View(studentQuiz);
        }

        // GET: StudentQuizAnswares/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentQuiz studentQuiz = db.StudentQuizs.Find(id);
            if (studentQuiz == null)
            {
                return HttpNotFound();
            }
            return View(studentQuiz);
        }

        // POST: StudentQuizAnswares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentQuiz studentQuiz = db.StudentQuizs.Find(id);
            db.StudentQuizs.Remove(studentQuiz);
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
