using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    public class LessonViewsController : Controller
    {
        private LearningManagementEntities db = new LearningManagementEntities();

        // GET: LessonViews
        public ActionResult Index()
        {
            var lessonViews = db.LessonViews.ToList();
            string user = User.Identity.GetUserId();
            if (User.IsInRole("Employee"))
            {
                lessonViews = db.LessonViews.Where(lv => lv.UserId == user).ToList();

            } else if (User.IsInRole("Admin") || User.IsInRole("HRAdmin") || User.IsInRole("Manager"))
            {
                lessonViews = db.LessonViews.ToList();

            } else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(lessonViews.ToList());
        }


        // GET: LessonViews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);

            string user = lessonView.UserId;

            var totalComplete = db.LessonViews.Include(c => c.Lesson).Include(c => c.UserDetail).Where(x => x.UserId == user).ToList();
            ViewBag.NbrCompleted = totalComplete.Count;

            if (lessonView == null)
            {
                return HttpNotFound();
            }
            return View(lessonView);
        }

        // GET: LessonViews/Create
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Create()
        {
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle");
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName");
            return View();
        }

        // POST: LessonViews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Create([Bind(Include = "LessonViewId,UserId,LessonId,DateViewed")] LessonView lessonView)
        {
            if (ModelState.IsValid)
            {
                db.LessonViews.Add(lessonView);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle", lessonView.LessonId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", lessonView.UserId);
            return View(lessonView);
        }

        // GET: LessonViews/Edit/5
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);
            if (lessonView == null)
            {
                return HttpNotFound();
            }
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle", lessonView.LessonId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", lessonView.UserId);
            return View(lessonView);
        }

        // POST: LessonViews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "HRAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonViewId,UserId,LessonId,DateViewed")] LessonView lessonView)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lessonView).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle", lessonView.LessonId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", lessonView.UserId);
            return View(lessonView);
        }

        // GET: LessonViews/Delete/5
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);
            if (lessonView == null)
            {
                return HttpNotFound();
            }
            return View(lessonView);
        }

        // POST: LessonViews/Delete/5
        [Authorize(Roles = "HRAdmin, Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LessonView lessonView = db.LessonViews.Find(id);
            db.LessonViews.Remove(lessonView);
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
