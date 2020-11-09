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
    public class stdAssignmentController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: stdAssignment
        public ActionResult Index(int? classID, int? courseID, int? TeacherID)
        { if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

           ViewBag.Studentid = int.Parse(Session["userID"].ToString());
            ViewBag.courseID = courseID;
            ViewBag.classID = classID;
            ViewBag.TeacherID = TeacherID;
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();

            var assigmnets = db.Assignments.Where(e=>periodIDs.Contains(e.PeriodID))
                .Where(e => e.ClassID == classID && e.CourseID == courseID && e.TeacherID == TeacherID)


                .Include(q => q.Class).Include(q => q.Cours).Include(q => q.User);
            return View(assigmnets.ToList().Where(e=>Functions.ChangeTime(e.EndDate.Value, 23, 59, 0, 0)
            > DateTime.Now));

        }




        public ActionResult Submited(int? classID, int? courseID, int? TeacherID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int id = int.Parse(Session["userID"].ToString());
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();

            var assigmnets = db.StudentAssignments.Where(e => periodIDs.Contains(e.PeriodID))
                .Where(e => e.StudentID== id && e.ClassID == classID && e.CourseID == courseID && e.TeacherID == TeacherID)
                 .Include(q => q.Class).Include(q => q.Cours).Include(q => q.User).Include(q=>q.Assignment);
            return View(assigmnets.ToList());

        }




        [HttpPost]
        public ActionResult CreateAssigment([Bind(Include = "ID,ClassID,CourseID,TeacherID,StudentID,AssignmentID,PDF")] StudentAssignment studentAssignment, HttpPostedFileBase ImageFile)
        {

            StudentAssignment studentAssignments = db.StudentAssignments
                .Where(e=>e.CourseID== studentAssignment.CourseID && e.ClassID== studentAssignment.ClassID && e.StudentID== studentAssignment.StudentID && e.TeacherID== studentAssignment.TeacherID).FirstOrDefault();

            if (studentAssignments != null)
            {


                if (ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/PDF/"), fileName);
                    ImageFile.SaveAs(path);
                    studentAssignments.PDF = "/Content/uploads/PDF/" + fileName;
                }
                var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).FirstOrDefault();
                studentAssignments.PeriodID = periodIDs.ID;
                db.Entry(studentAssignments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "stdAssignment", new { assignmentID = studentAssignments.AssignmentID, coursID = studentAssignments.CourseID, classID = studentAssignments.ClassID });

            }
            else {
                if (ImageFile.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/PDF/"), fileName);
                    ImageFile.SaveAs(path);
                    studentAssignments.PDF = "/Content/uploads/PDF/" + fileName;

                }
                var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).FirstOrDefault();
                studentAssignments.PeriodID = periodIDs.ID;
                studentAssignments.ClassID = studentAssignment.ClassID;
                studentAssignments.CourseID = studentAssignment.CourseID;
                studentAssignments.TeacherID = studentAssignment.TeacherID;
                studentAssignments.StudentID = studentAssignment.StudentID;
                studentAssignments.AssignmentID = studentAssignment.AssignmentID;
                db.StudentAssignments.Add(studentAssignments);
                db.SaveChanges();
                return RedirectToAction("Index", "stdAssignment", new { assignmentID = studentAssignments.AssignmentID, coursID = studentAssignments.CourseID, classID = studentAssignments.ClassID });





            }

        }




      
        public ActionResult stdAssigmnets(int? classID, int? courseID, int? TeacherID)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int id = int.Parse(Session["userID"].ToString());


            var assigmnets = db.Assignments
                .Where(e => e.ClassID == classID && e.CourseID == courseID && e.TeacherID == TeacherID)


                .Include(q => q.Class).Include(q => q.Cours).Include(q => q.User);
            return View(assigmnets.ToList().Where(e => Functions.ChangeTime(e.EndDate.Value, 23, 59, 0, 0)
            > DateTime.Now));

        }




        public ActionResult Acourses()
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

        // GET: stdAssignment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentAssignment studentAssignment = db.StudentAssignments.Find(id);
            if (studentAssignment == null)
            {
                return HttpNotFound();
            }
            return View(studentAssignment);
        }

        // GET: stdAssignment/Create
        public ActionResult Create()
        {
            ViewBag.AssignmentID = new SelectList(db.Assignments, "ID", "Title");
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name");
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: stdAssignment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClassID,CourseID,TeacherID,StudentID,AssignmentID,PDF,Grade,PeriodID")] StudentAssignment studentAssignment)
        {
            if (ModelState.IsValid)
            {
                db.StudentAssignments.Add(studentAssignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignmentID = new SelectList(db.Assignments, "ID", "Title", studentAssignment.AssignmentID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", studentAssignment.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", studentAssignment.CourseID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", studentAssignment.StudentID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentAssignment.TeacherID);
            return View(studentAssignment);
        }

        // GET: stdAssignment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentAssignment studentAssignment = db.StudentAssignments.Find(id);
            if (studentAssignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignmentID = new SelectList(db.Assignments, "ID", "Title", studentAssignment.AssignmentID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", studentAssignment.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", studentAssignment.CourseID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", studentAssignment.StudentID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentAssignment.TeacherID);
            return View(studentAssignment);
        }

        // POST: stdAssignment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClassID,CourseID,TeacherID,StudentID,AssignmentID,PDF,Grade,PeriodID")] StudentAssignment studentAssignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignmentID = new SelectList(db.Assignments, "ID", "Title", studentAssignment.AssignmentID);
            ViewBag.ClassID = new SelectList(db.Classes, "ID", "Name", studentAssignment.ClassID);
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", studentAssignment.CourseID);
            ViewBag.StudentID = new SelectList(db.Users, "ID", "Name", studentAssignment.StudentID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentAssignment.TeacherID);
            return View(studentAssignment);
        }

        // GET: stdAssignment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentAssignment studentAssignment = db.StudentAssignments.Find(id);
            if (studentAssignment == null)
            {
                return HttpNotFound();
            }
            return View(studentAssignment);
        }

        // POST: stdAssignment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentAssignment studentAssignment = db.StudentAssignments.Find(id);
            db.StudentAssignments.Remove(studentAssignment);
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
