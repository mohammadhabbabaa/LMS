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
    public class StdStudentAnswaresController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdStudentAnswares
        public ActionResult Index()
        {
            var studentQuizs = db.StudentQuizs.Include(s => s.Question).Include(s => s.Quiz).Include(s => s.User);
            return View(studentQuizs.ToList());
        }
        public ActionResult ExmFinish (int? quizID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            
            int id =  int.Parse(Session["userID"].ToString());
            var studentAnswares = db.StudentQuizs.Where(e => e.QuizID == quizID).Where(e => e.StudentID == id).ToList();
            ViewBag.QuizID = quizID;

            ViewBag.StudentID = id;
            return View(studentAnswares);

        }
        [HttpPost]
        public ActionResult Finish(Note notes,int? quizID)
        {

            Note nt = db.Notes.Where(e => e.StudentID == notes.StudentID)
                .Where(e => e.QuizID == notes.QuizID)
                .FirstOrDefault();

            if (nt != null)
            {
                nt.Note1 = notes.Note1;
                db.Entry(nt).State = EntityState.Modified;
                db.SaveChanges();
            }
            else { 
            if (ModelState.IsValid)
            {
                db.Notes.Add(notes);
                db.SaveChanges();

             }
            }
            return RedirectToAction("Index", "StdClasses");
        }

        // GET: StdStudentAnswares/Details/5
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

        // GET: StdStudentAnswares/Create
        public ActionResult Create(int? QuizID,int? QustionID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }


            Quiz quiz = db.Quizs.Find(QuizID);
            if (QustionID==null)
            {
                Question question = db.Questions.Where(e=>e.QuizID==QuizID).First();
                ViewBag.QustionID = question.ID;
                ViewBag.Puan = question.Puan;
                ViewBag.RightAnswer=question.RightAnswer;
                ViewBag.Image=question.Image;
            }
            else
            {
                Question question = db.Questions.Find(QustionID);
                ViewBag.QustionID = question.ID;
                ViewBag.Puan = question.Puan;
                ViewBag.RightAnswer=question.RightAnswer;
                ViewBag.Image=question.Image;
                var prvquistion = db.Questions.Where(e => e.ID < QustionID).Where(e => e.QuizID == QuizID).OrderByDescending(e => e.ID).FirstOrDefault();
                if (prvquistion!=null)
                {
                    ViewBag.Prevquestion = prvquistion.ID;

                }
               



            }

            string QuizEndHour = DateTime.Parse(quiz.EndHour).ToString("HH");
            string QuizEndMinute = DateTime.Parse(quiz.EndHour).ToString("mm");
            if (Functions.ChangeTime(quiz.Date.Value, int.Parse(QuizEndHour), int.Parse(QuizEndMinute), 0, 0) < DateTime.Now)
            {
                return RedirectToAction("ExmFinish", "StdStudentAnswares", new { quizID = QuizID });

            }



            ViewBag.QuizID = QuizID;
            ViewBag.StudentID = Session["userID"].ToString();





            return View();
        }

        // POST: StdStudentAnswares/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StudentID,QuizID,QustionID,Answer,Puan,RightAnswer")] StudentQuiz studentQuiz)
        {

            StudentQuiz stdquiz = db.StudentQuizs.Where(e=>e.StudentID==studentQuiz.StudentID)
                .Where(e=>e.QuizID==studentQuiz.QuizID)
                .Where(e=>e.QustionID==studentQuiz.QustionID)
                .FirstOrDefault();

            if (stdquiz!=null)
            {
                stdquiz.Answer = studentQuiz.Answer;
                db.Entry(stdquiz).State = EntityState.Modified;
                db.SaveChanges();
            }
            else {
                if (ModelState.IsValid)
                {
                    db.StudentQuizs.Add(studentQuiz);
                    db.SaveChanges();

                }
             
            }
            Question question = db.Questions.Where(e => e.ID > studentQuiz.QustionID).Where(e => e.QuizID == studentQuiz.QuizID).FirstOrDefault();

            if (question == null)
            {
                return RedirectToAction("ExmFinish", "StdStudentAnswares", new { quizID = studentQuiz.QuizID });
            }
            else
            {
                return RedirectToAction("Create", "StdStudentAnswares", new { QuizID = studentQuiz.QuizID, QustionID = question.ID });

            }

     
        }

          


        // GET: StdStudentAnswares/Edit/5
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

        // POST: StdStudentAnswares/Edit/5
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

        // GET: StdStudentAnswares/Delete/5
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

        // POST: StdStudentAnswares/Delete/5
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
