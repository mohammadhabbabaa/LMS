using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kurs.Models;
using PagedList;
using PagedList.Mvc;


namespace Kurs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Teachers
        public ActionResult Index(int? page ,string search )
        {
            var users = db.Users.Include(u => u.Role).Where(e => e.Active == 1).Where(e => e.UserTaype == 3).Where(e => e.Name.StartsWith(search) || search == null).OrderByDescending(e => e.ID);
            return View(users.ToList().ToPagedList(page ?? 1, 40));
        }

        // GET: Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            ViewBag.UserTaype = new SelectList(db.Roles, "ID", "Name");
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Emial,Birthday,Address,Phone")] User user)
        {
            if (Functions.IsEmailExist(user.Emial))
            {
                ModelState.AddModelError("Email Error", "البريد الالكتروني مستخدم");
                return View();
            }
            else
            {
                if (ModelState.IsValid)
            {
                user.Password = "";
                user.Active = 1;
                user.UserTaype = 3;

                user.Registeration = DateTime.Now.Date;
                db.Users.Add(user);
                db.SaveChanges();
                    TempData["success"] = "asdasd";

                    return RedirectToAction("Create", "StudentClasses", new { id = user.ID });
                }

            ViewBag.UserTaype = new SelectList(db.Roles, "ID", "Name", user.UserTaype);
                TempData["error"] = "asdasd";
                return View(user);
        }
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserTaype = new SelectList(db.Roles, "ID", "Name", user.UserTaype);
            return View(user);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Emial,Birthday,Address,Phone")] User user)
        {
            if (ModelState.IsValid)
            {
                User myuser = db.Users.Find(user.ID);
                myuser.Name = user.Name;
                myuser.Emial = user.Emial;
                myuser.Birthday = user.Birthday;
                myuser.Address = user.Address;
                myuser.Phone = user.Phone;
        

                db.Entry(myuser).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index");
            }
            ViewBag.UserTaype = new SelectList(db.Roles, "ID", "Name", user.UserTaype);
            TempData["error"] = "asdasd";
            return View(user);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User teacher = db.Users.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            teacher.Active = 0;
            db.Entry(teacher).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "asdasd";
            return RedirectToAction("Index");
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
