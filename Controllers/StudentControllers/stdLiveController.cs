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

namespace Kurs.Controllers.StudentControllers
{
    public class stdLiveController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: stdLive
        public ActionResult Index( )
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
       
            int TeacherID = int.Parse(Session["TeacherID"].ToString());
            int classID = int.Parse(Session["classID"].ToString());
            int courseID = int.Parse(Session["courseID"].ToString());
             LiveCours liveCourses = db.LiveCourses.Where(e=>e.TeacherID== TeacherID && e.ClassID== classID && e.CourseID== courseID)
                .Include(l => l.Class).Include(l => l.Cours).Include(l => l.User).First();
            return View(liveCourses);
        }

       
       
        public ActionResult setSessions(int? classID ,int? courseID ,int? TeacherID )
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
         
            int id = int.Parse(Session["userID"].ToString());
           User  teacher = db.Users.Where(e => e.ID == TeacherID).FirstOrDefault();
            Session["API_KEY"] = teacher.API_KEY;
            Session["API_SECRET"] = teacher.API_SECRET;
            Session["TeacherID"] = TeacherID;
            Session["courseID"] = courseID;
            Session["classID"] = classID;
            Session["StdName"] = db.Users.Where(e => e.ID == id).Select(e => e.Name).First();
         
            return RedirectToAction("Index");
        }


        public ActionResult courses()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            var studentClasses = db.StudentClasses.Where(e => periodIDs.Contains(e.PeriodID)).Where(e => e.UserID== id)
               .Include(l => l.Class).Include(l => l.Cours).Include(l => l.User);
            return View(studentClasses.ToList());
        }





        // GET: stdLive/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveCours liveCours = db.LiveCourses.Find(id);
            if (liveCours == null)
            {
                return HttpNotFound();
            }
            return View(liveCours);
        }

        // GET: stdLive/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: stdLive/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClassID,CourseID,TeacherID,ZoomUser,ZoomPass")] LiveCours liveCours)
        {
            if (ModelState.IsValid)
            {
                db.LiveCourses.Add(liveCours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", liveCours.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", liveCours.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", liveCours.TeacherID);
            return View(liveCours);
        }

        // GET: stdLive/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveCours liveCours = db.LiveCourses.Find(id);
            if (liveCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", liveCours.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", liveCours.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", liveCours.TeacherID);
            return View(liveCours);
        }

        // POST: stdLive/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClassID,CourseID,TeacherID,ZoomUser,ZoomPass")] LiveCours liveCours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liveCours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", liveCours.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", liveCours.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", liveCours.TeacherID);
            return View(liveCours);
        }

        // GET: stdLive/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveCours liveCours = db.LiveCourses.Find(id);
            if (liveCours == null)
            {
                return HttpNotFound();
            }
            return View(liveCours);
        }

        // POST: stdLive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiveCours liveCours = db.LiveCourses.Find(id);
            db.LiveCourses.Remove(liveCours);
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
