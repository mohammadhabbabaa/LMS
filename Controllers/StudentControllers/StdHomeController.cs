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
    public class StdHomeController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StdHome
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int id = int.Parse(Session["userID"].ToString());
            var StdCourses = db.StudentClasses
           .Where(e => e.UserID == id).ToArray().Select(e => e.CoursID);



            var notifications = db.Notifications
                 .Where(e => e.EndDate > DateTime.Now)
                 .Where(e => e.ToUserType == 2)
              .Where(p => StdCourses.Contains(p.CoursID))
                .Include(n => n.Cours).Include(n => n.Role).Include(n => n.User);

            return View(notifications.ToList());


        }

        // GET: StdHome/Details/5
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

        // GET: StdHome/Create
        public ActionResult Create()
        {
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name");
            ViewBag.FromID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: StdHome/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FromID,CoursID,Title,Description,EndDate,ToUserType")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name", notification.CoursID);
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name", notification.ToUserType);
            ViewBag.FromID = new SelectList(db.Users, "ID", "Name", notification.FromID);
            return View(notification);
        }

        // GET: StdHome/Edit/5
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
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name", notification.CoursID);
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name", notification.ToUserType);
            ViewBag.FromID = new SelectList(db.Users, "ID", "Name", notification.FromID);
            return View(notification);
        }

        // POST: StdHome/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FromID,CoursID,Title,Description,EndDate,ToUserType")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CoursID = new SelectList(db.Courses, "ID", "Name", notification.CoursID);
            ViewBag.ToUserType = new SelectList(db.Roles, "ID", "Name", notification.ToUserType);
            ViewBag.FromID = new SelectList(db.Users, "ID", "Name", notification.FromID);
            return View(notification);
        }

        // GET: StdHome/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: StdHome/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
