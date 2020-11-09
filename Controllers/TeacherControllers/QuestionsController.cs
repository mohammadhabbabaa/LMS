using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class QuestionsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Questions
        public ActionResult Index(int ? QuizID)
        {
            ViewBag.QuizID = QuizID;
            var questions = db.Questions.Where(e=>e.QuizID == QuizID).Include(q => q.Quiz);
            var myQuiz = db.Quizs.Where(e => e.ID == QuizID).FirstOrDefault();
            ViewBag.CoursID = myQuiz.CourseID;
            ViewBag.ClassID = myQuiz.ClassID;
    
            return View(questions.ToList());
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create(int? QuizID)
        {
            ViewBag.QuizID = QuizID;
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Puan,QuizID,Image,RightAnswer")] Question question, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/Quizs/"), fileName);
                    ImageFile.SaveAs(path);
                    question.Image = "/Content/uploads/Quizs/" + fileName;
                }
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Create", "Questions", new { QuizID= question.QuizID});
            }

            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", question.QuizID);
            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", question.QuizID);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Puan,QuizID,Image,RightAnswer")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "sdasdsa";
                return RedirectToAction("Index", "Questions", new { QuizID = question.QuizID });
            }
            ViewBag.QuizID = new SelectList(db.Quizs, "ID", "Title", question.QuizID);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            int qID = question.QuizID.Value;
            if (question == null)
            {
                return HttpNotFound();
            }
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index", "Questions", new { QuizID = qID });
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
