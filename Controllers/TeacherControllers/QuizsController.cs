using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;
using Microsoft.Ajax.Utilities;

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class QuizsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Quizs
        public ActionResult Index(int? coursID, int? classID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.CourseID = coursID;
            ViewBag.classID = classID;
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            return View(db.Quizs.Where(e=>periodIDs.Contains(e.PeriodID)).Where(e => e.CourseID == coursID && e.ClassID == classID && e.TeacherID == id)
                  .Include(a => a.Cours).Include(a => a.User)
                .OrderByDescending(e => e.ID).
             ToList());
        }




        public ActionResult Quizs()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int id = int.Parse(Session["userID"].ToString());
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            return View(db.TeachersDates.Where(e=>periodIDs.Contains(e.PeriodID)).Where(e => e.TeacherID == id).OrderBy(m => m.CourseID)
                .DistinctBy(m => new { m.CourseID, m.ClassID }).ToList());
        }
        // GET: Quizs/Details/5
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

        // GET: Quizs/Create
        public ActionResult Create(int? coursID, int? classID)
        {
            ViewBag.CourseID = coursID;
            ViewBag.classID = classID;
            return View();
        }

        // POST: Quizs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,CourseID,TeacherID,ClassID,Date,StratHour,EndHour")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
            
                var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).FirstOrDefault();
                quiz.PeriodID = periodIDs.ID;
                db.Quizs.Add(quiz);
                db.SaveChanges();
                TempData["success"] = "asdas";
                return RedirectToAction("Create", "Questions", new { QuizID = quiz.ID });
            }

            return View(quiz);
        }

        // GET: Quizs/Edit/5
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
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", quiz.CourseID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", quiz.ClassID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", quiz.TeacherID);
            return View(quiz);
        }

        // POST: Quizs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,CourseID,TeacherID,ClassID,Date,StratHour,EndHour")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).FirstOrDefault();
                quiz.PeriodID = periodIDs.ID;
                db.Entry(quiz).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdas";
                return RedirectToAction("Index", "Quizs", new { coursID = quiz.CourseID, classID = quiz.ClassID });
            }
            return View(quiz);
        }

        // GET: Quizs/Delete/5
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
            int coursID = quiz.CourseID.Value;
            int classID = quiz.ClassID.Value;
            db.Quizs.Remove(quiz);
            db.SaveChanges();
            TempData["success"] = "asdasd";

            return RedirectToAction("Index", "Quizs", new { coursID = coursID, classID = classID });
        }

        // POST: Quizs/Delete/5
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
