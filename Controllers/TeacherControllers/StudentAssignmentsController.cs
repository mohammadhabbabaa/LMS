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

namespace Kurs.Controllers.TeacherControllers
{
    [Authorize(Roles = "Teacher")]
    public class StudentAssignmentsController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: StudentAssignments
        public ActionResult Index(int? coursID, int? classID,int? AssignmentID, int? page, string search)
        {

            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var periodIDs = db.Periods.Where(e => e.EndDate >= DateTime.Now).Select(e => e.ID).ToArray();
            ViewBag.CourseID = coursID;
            ViewBag.classID = classID;
            ViewBag.AssignmentID = AssignmentID;
            // the logged user ID = Teacher 
            int id = int.Parse(Session["userID"].ToString());

            // All Student Info  who sent Assignment 
            var studentAssignments = db.StudentAssignments.Where(e=>periodIDs.Contains(e.PeriodID))
                .Where(e => e.CourseID == coursID && e.ClassID == classID && e.TeacherID == id && e.AssignmentID == AssignmentID)
              .Where(e => e.User.Name.StartsWith(search) || search == null)
                .Include(s => s.Cours).Include(s => s.User).Include(s => s.User1);

         

            // Compering dates to now if the assignment date finish make 0 to not student not sent it
            var AssignmentDate = db.Assignments.Where(e=>e.ID==AssignmentID).Select(e => e.EndDate).First();
            DateTime AssignmentEendDate = DateTime.Parse(AssignmentDate.ToString()) ;
            string Today = DateTime.Now.ToString("M/d/yyyy");
              DateTime  Todaydt = DateTime.Parse(Today);

            // the IDs of Student Sent Assignment 
            var SentstudentsID = studentAssignments.Select(e => e.StudentID).ToArray();

            if (Todaydt.Date > AssignmentEendDate.Date)
            {
            
                var StudenNotSentAssignmnet = db.StudentClasses.Where(e=>periodIDs.Contains(e.PeriodID))
             .Where(e => e.CoursID == coursID && e.ClassID == classID && e.TeacherID == id)
             .Where(p => !SentstudentsID.Contains(p.UserID))
                .ToList()
           ;
                foreach (var value in StudenNotSentAssignmnet)
                {
                    StudentAssignment std = new StudentAssignment();
                    std.StudentID = value.UserID;
                    std.ClassID = classID;
                    std.CourseID = coursID;
                    std.AssignmentID = AssignmentID;
                    std.TeacherID = id;
                    std.Grade = "0";
                    db.StudentAssignments.Add(std);
              
                }
                db.SaveChanges();

            }






            return View(studentAssignments.ToList().ToPagedList(page ?? 1, 20));
        }

        // GET: StudentAssignments/Details/5
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

        // GET: StudentAssignments/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
            ViewBag.StudentId = new SelectList(db.Users, "ID", "Name");
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: StudentAssignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StudentId,TeacherID,CourseID,PDF")] StudentAssignment studentAssignment)
        {
            if (ModelState.IsValid)
            {
                db.StudentAssignments.Add(studentAssignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", studentAssignment.CourseID);
            ViewBag.StudentId = new SelectList(db.Users, "ID", "Name", studentAssignment.StudentID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentAssignment.TeacherID);
            return View(studentAssignment);
        }



        public ActionResult CreateGrade([Bind(Include = "ID,Grade")] StudentAssignment studentAssignment)
        {

            StudentAssignment studentAssignments = db.StudentAssignments.Find(studentAssignment.ID);
            studentAssignments.Grade = studentAssignment.Grade;
            db.Entry(studentAssignments).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "StudentAssignments", new { assignmentID = studentAssignments.AssignmentID ,coursID = studentAssignments.CourseID , classID = studentAssignments.ClassID  });

        }










        // GET: StudentAssignments/Edit/5
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
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", studentAssignment.CourseID);
            ViewBag.StudentId = new SelectList(db.Users, "ID", "Name", studentAssignment.StudentID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentAssignment.TeacherID);
            return View(studentAssignment);
        }

        // POST: StudentAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StudentId,TeacherID,CourseID,PDF")] StudentAssignment studentAssignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", studentAssignment.CourseID);
            ViewBag.StudentId = new SelectList(db.Users, "ID", "Name", studentAssignment.StudentID);
            ViewBag.TeacherID = new SelectList(db.Users, "ID", "Name", studentAssignment.TeacherID);
            return View(studentAssignment);
        }

        // GET: StudentAssignments/Delete/5
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

        // POST: StudentAssignments/Delete/5
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
