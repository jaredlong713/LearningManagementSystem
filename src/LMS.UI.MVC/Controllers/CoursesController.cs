using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;

namespace LMS.UI.MVC.Controllers
{
    public class CoursesController : Controller
    {
        private LearningManagementEntities db = new LearningManagementEntities();

        // GET: Courses
        public ActionResult Index(string isActive)
        {
            if (string.IsNullOrEmpty(isActive) || isActive == "active")
            {
                ViewBag.IsActive = true;
                return View(db.Courses.Where(c => c.IsActive == true).ToList());
            } else if (isActive == "inactive")
            {
                ViewBag.IsActive = false;
                return View(db.Courses.Where(c => c.IsActive == false).ToList());
            }
            return View(db.Courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseDescription,CourseTag,CourseTime,IsActive")] Course course, HttpPostedFileBase coursePhoto)
        {
            if (ModelState.IsValid)
            {
                string image = string.Empty;

                if(coursePhoto != null)
                {
                    image = coursePhoto.FileName;
                    string ext = image.Substring(image.LastIndexOf("."));
                    string[] okExtentions = { ".jpg", ".jpeg", ".png" };

                    if (okExtentions.Contains(ext.ToLower()))
                    {
                        image = Guid.NewGuid() + ext;

                        coursePhoto.SaveAs
                            (Server.MapPath("~/Content/img/courses/" + image));
                    }

                    course.CourseImage = image;
                }

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseDescription,CourseTag,CourseTime,IsActive")] Course course, HttpPostedFileBase coursePhoto)
        {
            if (ModelState.IsValid)
            {
                string image = string.Empty;

                if (coursePhoto != null)
                {
                    image = coursePhoto.FileName;
                    string ext = image.Substring(image.LastIndexOf("."));
                    string[] okExtentions = { ".jpg", ".jpeg", ".png" };

                    if (okExtentions.Contains(ext.ToLower()))
                    {
                        image = Guid.NewGuid() + ext;

                        coursePhoto.SaveAs
                            (Server.MapPath("~/Content/img/courses/" + image));
                    }

                    course.CourseImage = image;
                }


                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);

            if (course.IsActive)
            {
                course.IsActive = false;

            } else
            {
                course.IsActive = true;
            }

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
