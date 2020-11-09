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
    public class ZoomController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Zoom
        public ActionResult Index()
        {
            var liveCourses = db.LiveCourses.Include(l => l.Cours).Include(l => l.User);
            return View(liveCourses.ToList());
        }

        // GET: Zoom/Details/5
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

        // GET: Zoom/Create
        public ActionResult Create(int? coursID, int? classID)
        {
            ViewBag.coursID = coursID;
            ViewBag.classID = classID;
            return View();
        }

        // POST: Zoom/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,TeacherID,ZoomUser,ZoomPass,ClassID")] LiveCours liveCours)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int id = int.Parse(Session["userID"].ToString());
            LiveCours myliveCours = db.LiveCourses.Where(e=>e.ClassID==liveCours.ClassID && e.CourseID==liveCours.CourseID && e.TeacherID==liveCours.TeacherID).FirstOrDefault();
            if (myliveCours == null)
            {
                if (ModelState.IsValid)
                {
                    db.LiveCourses.Add(liveCours);
                    db.SaveChanges();
                    return RedirectToAction("Index", "TeachersVideos", new { coursID = liveCours.CourseID, classID = liveCours.ClassID });
                }
            }
            else {

                if (ModelState.IsValid)
                {
                    myliveCours.ClassID = liveCours.ClassID;
                    myliveCours.CourseID = liveCours.CourseID;
                    myliveCours.TeacherID = liveCours.TeacherID;
                    myliveCours.ZoomUser = liveCours.ZoomUser;
                    myliveCours.ZoomPass = liveCours.ZoomPass;
                    db.Entry(myliveCours).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "TeachersVideos", new { coursID = liveCours.CourseID, classID = liveCours.ClassID });
                }

            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", liveCours.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", liveCours.TeacherID);
            return View(liveCours);
        }

        // GET: Zoom/Edit/5
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
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", liveCours.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", liveCours.TeacherID);
            return View(liveCours);
        }

        // POST: Zoom/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CourseID,TeacherID,ZoomUser,ZoomPass")] LiveCours liveCours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liveCours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", liveCours.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", liveCours.TeacherID);
            return View(liveCours);
        }

        // GET: Zoom/Delete/5
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

        // POST: Zoom/Delete/5
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
