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
using PagedList.Mvc;

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class TeachersVideosController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: TeachersVideos
        public ActionResult Index(int? coursID ,int? classID, int? page)
        {
            if (Session["userID"]== null)
            {
                return RedirectToAction("Login","Login");
            }
            int id = int.Parse(Session["userID"].ToString());
            ViewBag.coursID = coursID;
            ViewBag.classID = classID;
            var videos = db.Videos.Where(e=>e.CourseID==coursID && e.ClassID==classID && e.TeacherID==id).Include(v => v.Cours).Include(v => v.User).OrderByDescending(e=>e.ID);
            return View(videos.ToList().ToPagedList(page ?? 1, 10));
        }

        // GET: TeachersVideos/Details/5
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

        // GET: TeachersVideos/Create
        public ActionResult Create(int? coursID, int? classID)
        {
            ViewBag.coursID = coursID;
            ViewBag.classID = classID;
            return View();
        }

        // POST: TeachersVideos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,Image,TeacherID,CourseID,Video1,ClassID")] Video video, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/Videos/"), fileName);
                    ImageFile.SaveAs(path);
                    video.Video1 = "/Content/uploads/Videos/" + fileName;
                }
                db.Videos.Add(video);
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", video.CourseID);
            ViewBag.ClassID = new SelectList(db.Courses, "ID", "Name", video.ClassID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", video.TeacherID);
            return View(video); 
        }

        // GET: TeachersVideos/Edit/5
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
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", video.CourseID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", video.ClassID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", video.TeacherID);
            return View(video);
        }

        // POST: TeachersVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,ClassID,TeacherID,CourseID,Video1")] Video video, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/Videos/"), fileName);
                    ImageFile.SaveAs(path);
                    video.Video1 = "/Content/uploads/Videos/" + fileName;
                }
                int classID = video.ClassID.Value;
                int CoursID = video.CourseID.Value;
                db.Videos.Attach(video);
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index", new { classID = classID, coursID = CoursID });
        
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", video.CourseID);
            ViewBag.ClassID = new SelectList(db.Courses, "ID", "Name", video.ClassID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", video.TeacherID);
            return View(video);
        }

        // GET: TeachersVideos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            string strPhysicalFolder = Server.MapPath("~/");

            string strFileFullPath = strPhysicalFolder + video.Video1;

            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
            }
       
        int classID =  video.ClassID.Value;
         int CoursID = video.CourseID.Value;
            db.Videos.Remove(video);
            db.SaveChanges();
            TempData["success"] = "asdasd";
            return RedirectToAction("Index",new { classID = classID, coursID= CoursID });
        
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
