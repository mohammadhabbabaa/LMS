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
using PagedList;

namespace Kurs.Controllers.StudentControllers
{
    public class StdVideosController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdVideos
        public ActionResult Index(int? coursID, int? classID, int? page)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int id = int.Parse(Session["userID"].ToString());
            var studentClasses = db.StudentClasses
                .Where(e => e.UserID == id && e.ClassID == classID && e.CoursID == coursID).FirstOrDefault();
               
              
            if (studentClasses == null)
            {
                return HttpNotFound();
            }
            else { 

            ViewBag.coursID = coursID;
            ViewBag.classID = classID;
            var videos = db.Videos.Where(e => e.CourseID == coursID && e.ClassID == classID).Include(v => v.Cours).Include(v => v.User).OrderByDescending(e => e.ID);
            return View(videos.ToList().ToPagedList(page ?? 1, 9));
        }
        }


        public ActionResult Display(int? id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

     
            var videos = db.Videos.Where(e => e.ID==id).Include(v => v.Cours).Include(v => v.User).FirstOrDefault();
            return View(videos);
        }


        public ActionResult GetVideo(int ? id)
        {
            var videos = db.Videos.Where(e => e.ID == id).Include(v => v.Cours).Include(v => v.User).FirstOrDefault();
            var videoPath = Request.MapPath(videos.Video1);
            FileStream fs = new FileStream(videoPath, FileMode.Open);
            return new FileStreamResult(fs, "video/mp4");
        }






        // GET: StdVideos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // GET: StdVideos/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: StdVideos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description,TeacherID,CourseID,Video1,ClassID")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Videos.Add(video);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", video.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", video.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", video.TeacherID);
            return View(video);
        }

        // GET: StdVideos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", video.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", video.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", video.TeacherID);
            return View(video);
        }

        // POST: StdVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,TeacherID,CourseID,Video1,ClassID")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", video.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", video.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", video.TeacherID);
            return View(video);
        }

        // GET: StdVideos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: StdVideos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Video video = db.Videos.Find(id);
            db.Videos.Remove(video);
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
