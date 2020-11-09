using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;

using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Kurs.Models;


namespace Kurs.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class NotificationsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Notifications
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int id = int.Parse(Session["userID"].ToString());
            var notifications = db.Notifications.Include(e=>e.Cours).Where(e=>e.FromID ==id );
            return View(notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
           
            if (int.Parse(Session["UserType"].ToString()) == 2)
            {
                int id = int.Parse(Session["userID"].ToString());
                var coursid = db.TeachersDates.Where(e=>e.TeacherID== id).Select(e=>e.CourseID);
                ViewBag.CoursID = new SelectList(db.Courses.Where(e => e.Active == 1 && coursid.Any(i => e.ID == i)), "ID", "Name");

            }
           else if (int.Parse(Session["UserType"].ToString()) == 1)
            {

                ViewBag.CoursID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name");

            }
            else {
                ViewBag.CoursID = "";


            }
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CoursID,Title,Description,EndDate,ToUserType")] Notification notification)
        {
            if (ModelState.IsValid)
            {
               notification.FromID = int.Parse(Session["userID"].ToString());
         
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name");
            ViewBag.CoursID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name");
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name");
            ViewBag.CoursID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name");
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CoursID,Title,Description,EndDate,ToUserType")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.FromID = int.Parse(Session["userID"].ToString());
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name");
            ViewBag.CoursID = new SelectList(db.Courses.Where(e => e.Active == 1), "ID", "Name");
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
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
