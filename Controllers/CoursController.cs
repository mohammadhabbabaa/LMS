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

namespace Kurs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursController : Controller
    {
        private KursEntities db = new KursEntities();

        // GET: Cours
        public ActionResult Index()
        {
            return View(db.Courses.Where(e => e.Active == 1).OrderByDescending(e => e.ID).ToList());
        }

        // GET: Cours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // GET: Cours/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
   
        public ActionResult Create([Bind(Include = "ID,Name,Description,Image,End,Strat")] Cours cours, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss")+ ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    cours.Image = "/Content/uploads/Images/" + fileName;
                }
                cours.Active = 1;
                 db.Courses.Add(cours);
                 db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index");
            }
            TempData["error"] = "asdasd";
            return View(cours);
        }
       


        // GET: Cours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // POST: Cours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Image,End,Strat")] Cours cours, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    cours.Image = "/Content/uploads/Images/" + fileName;
                }
                cours.Active = 1;
                db.Entry(cours).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "asdasd";
                return RedirectToAction("Index");
            }
            TempData["error"] = "asdasd";
            return View(cours);
        }

        // GET: Cours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }


            cours.Active = 0;
            db.Entry(cours).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "asdasd";
            return RedirectToAction("Index");

     
        }

        // POST: Cours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cours cours = db.Courses.Find(id);
            db.Courses.Remove(cours);
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
