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
    public class StdQuizsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdQuizs
        public ActionResult Index(int? classID, int? courseID, int? TeacherID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int id = int.Parse(Session["userID"].ToString());

         
            var quizs = db.Quizs
                .Where(e=>e.ClassID==classID && e.CourseID ==courseID && e.TeacherID==TeacherID)
              
                
                .Include(q => q.Class).Include(q => q.Cours).Include(q => q.User);
            return View(quizs.ToList().Where(e =>
              Functions.ChangeTime(e.Date.Value, int.Parse(DateTime.Parse(e.StratHour).ToString("HH")), int.Parse(DateTime.Parse(e.StratHour).ToString("mm")), 0, 0)
              <= DateTime.Now)
              .Where(e =>
              Functions.ChangeTime(e.Date.Value, int.Parse(DateTime.Parse(e.EndHour).ToString("HH")), int.Parse(DateTime.Parse(e.EndHour).ToString("mm")), 0, 0)
              >= DateTime.Now));
        }

        public ActionResult qcourses()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            var studentClasses = db.StudentClasses.Where(e=>periodIDs.Contains(e.PeriodID)).Where(e => e.UserID == id).Include(s => s.Class).Include(s => s.Cours).Include(s => s.User).Include(s => s.User1).Include(s => s.Period);
            return View(studentClasses.ToList());
        }

        // GET: StdQuizs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizs.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // GET: StdQuizs/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: StdQuizs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClassID,CourseID,TeacherID,Title,Date,StratHour,EndHour,PeriodID")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                db.Quizs.Add(quiz);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", quiz.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", quiz.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", quiz.TeacherID);
            return View(quiz);
        }

        // GET: StdQuizs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizs.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", quiz.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", quiz.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", quiz.TeacherID);
            return View(quiz);
        }

        // POST: StdQuizs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClassID,CourseID,TeacherID,Title,Date,StratHour,EndHour,PeriodID")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quiz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", quiz.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", quiz.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", quiz.TeacherID);
            return View(quiz);
        }

        // GET: StdQuizs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizs.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: StdQuizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quiz quiz = db.Quizs.Find(id);
            db.Quizs.Remove(quiz);
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
