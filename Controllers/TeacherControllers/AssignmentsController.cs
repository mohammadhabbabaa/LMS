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
using Microsoft.Ajax.Utilities;

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class AssignmentsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Assignments
        public ActionResult Index(int? coursID, int? classID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.CourseID = coursID;
            ViewBag.classID = classID;
            int id = int.Parse(Session["userID"].ToString());



            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            var assignments = db.Assignments.Where(e=>periodIDs.Contains(e.PeriodID))
                .Where(e => e.CourseID == coursID && e.ClassID == classID && e.TeacherID == id)
                .Include(a => a.Cours).Include(a => a.User)
                .OrderByDescending(e=>e.ID);
            return View(assignments.ToList());
        }
        public ActionResult Assignments()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            int id = int.Parse(Session["userID"].ToString());
            return View(db.TeachersDates.Where(e=>periodIDs.Contains(e.PeriodID)).Where(e => e.TeacherID == id).OrderBy(m => m.CourseID)
                .DistinctBy(m => new { m.CourseID, m.ClassID }).ToList());
        }
        // GET: Assignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // GET: Assignments/Create
        public ActionResult Create(int? coursID, int? classID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.CourseID = coursID;
            ViewBag.classID = classID;
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TeacherID,CourseID,Title,Description,PDF,ClassID,EndDate")] Assignment assignment, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).FirstOrDefault();
                if (ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/PDF/"), fileName);
                    ImageFile.SaveAs(path);
                    assignment.PDF = "/Content/uploads/PDF/" + fileName;
                }
                assignment.PeriodID = periodIDs.ID;
                db.Assignments.Add(assignment);
                db.SaveChanges();
                return RedirectToAction("Index", "Assignments", new { coursID = assignment.CourseID, classID = assignment.ClassID });

            }

            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", assignment.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", assignment.TeacherID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", assignment.ClassID);
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", assignment.CourseID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", assignment.ClassID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", assignment.TeacherID);
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TeacherID,CourseID,Title,Description,PDF,EndDate,ClassID")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).FirstOrDefault();
                assignment.PeriodID = periodIDs.ID;
                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index", "Assignments", new { coursID = assignment.CourseID , classID = assignment.ClassID });
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", assignment.CourseID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", assignment.TeacherID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", assignment.ClassID);
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
           int coursID = assignment.CourseID.Value;
            int classID = assignment.ClassID.Value;
            db.Assignments.Remove(assignment);
            db.SaveChanges();
            TempData["success"] = "asdasd";

            return RedirectToAction("Index", "Assignments", new { coursID = coursID, classID = classID });

        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            db.Assignments.Remove(assignment);
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
