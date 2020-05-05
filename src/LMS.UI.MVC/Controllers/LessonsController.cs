using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    public class LessonsController : Controller
    {
        private LearningManagementEntities db = new LearningManagementEntities();

        // GET: Lessons
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Course).OrderBy(l => l.CourseId);
            return View(lessons.ToList());
        }

        //public ActionResult Index(int? id)
        //{
        //    ViewBag.Id = id;
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    else
        //    {
        //        var lesson = db.Lessons.Where(l => l.LessonId == id).SingleOrDefault();
        //        return View(lesson);
        //    }
        //}

        // GET: Lessons/Details/5
        public async System.Threading.Tasks.Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            string user = User.Identity.GetUserId();

            if (User.IsInRole("Employee"))
            {
                var viewedLessons = db.LessonViews.Where(l => l.UserId == user);
                bool hasViewed = false;

                foreach(var lessonItem  in viewedLessons)
                {
                    if(id == lessonItem.LessonId)
                    {
                        hasViewed = true;
                    }
                }

                if (!hasViewed)
                {
                    LessonView newLessonView = new LessonView();

                    newLessonView.UserId = user;
                    newLessonView.LessonId = (int)id;
                    newLessonView.DateViewed = DateTime.Now;
                    db.LessonViews.Add(newLessonView);
                    db.SaveChanges();
                }

                Course course = db.Courses.Find(lesson.CourseId);
                int courseLessons = db.Lessons.Where(l => l.CourseId == lesson.CourseId).Count();
                var lessonIds = db.Lessons.Where(l => l.CourseId == lesson.CourseId).Select(l => l.LessonId);
                var studentViewedLessons = 0;

                bool completedCourse = false;
                var completedCourses = db.CourseCompletions.Where(cc => cc.UserId == user);


                foreach(var l in db.LessonViews.Where(l => l.UserId == user))
                {
                    foreach(var lids in lessonIds)
                    {
                        if(l.LessonId == lids)
                        {
                            studentViewedLessons++;
                        }
                    }
                }

                foreach( var cc in completedCourses)
                {
                    if(cc.CourseId == lesson.CourseId)
                    {
                        completedCourse = true;
                    }
                }

                if(studentViewedLessons == courseLessons && !completedCourse)
                {
                    CourseCompletion cc = new CourseCompletion();
                    cc.UserId = user;
                    cc.CourseId = course.CourseId;
                    cc.DateCompleted = DateTime.Now;
                    db.CourseCompletions.Add(cc);
                    db.SaveChanges();


                    UserDetail userDetail = db.UserDetails.Where(ud => ud.UserId == user).SingleOrDefault();

                    string body = $"<p>On <strong>{cc.DateCompleted}</strong>, <strong>{userDetail.FirstName} {userDetail.LastName}</strong> completed the course <strong>{course.CourseName}</strong></p> ";

                    MailMessage msg = new MailMessage("no-reply@domain.com", "Jaredlong713@gmail.com", $"{userDetail.FirstName} {userDetail.LastName} completed a course!", body);

                    msg.IsBodyHtml = true;
                    using (var mailClient = new SmtpClient())
                    {

                        mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mailClient.UseDefaultCredentials = false;
                        mailClient.EnableSsl = true;
                        mailClient.Host = "smtp-mail.outlook.com";
                        mailClient.Port = 587;
                        mailClient.Credentials =
                                        new NetworkCredential(
                                            "jlong@centriq.com",
                                            "Password!");

                        try
                        {
                            await mailClient.SendMailAsync(msg);
                            ViewBag.MailSent =
                                "Email has been sent.";

                            return View(lesson);
                        }
                        catch
                        {
                            ViewBag.ErrorMessage = "Sorry, something went wrong.  We've recorded your progress, but please inform your manager that the course has been completed.";

                            return View(lesson);
                        }
                    }
                }
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "HRAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase pdfUpload)
        {
            if (ModelState.IsValid)
            {
                if(lesson.VideoURL != null)
                {
                    if (lesson.VideoURL.Contains("/watch?"))
                    {
                        lesson.VideoURL = lesson.VideoURL.Replace("/watch?v=", "/embed/");
                    }
                }

                if (pdfUpload != null)
                {
                    string filename = pdfUpload.FileName;

                    string ext = filename.Substring(filename.LastIndexOf("."));

                    string[] goodExts = { ".pdf" };

                    if (goodExts.Contains(ext))
                    {
                        pdfUpload.SaveAs(Server.MapPath("~/Content/resources/" + filename));
                        lesson.PdfFilename = filename;
                    }
                    else
                    {
                        ViewBag.CourseId = new SelectList(db.Courses, "CourseID", "CourseName", lesson.CourseId);
                        ViewBag.ErrorMessage = "* Only PDF files are allowed";
                        return View(lesson);
                    }

                }

                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View("Details", "Course");
        }

        // GET: Lessons/Edit/5
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "HRAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase pdfUpload)
        {
            if (ModelState.IsValid)
            {
                if (lesson.VideoURL != null)
                {
                    if (lesson.VideoURL.Contains("/watch?"))
                    {
                        lesson.VideoURL = lesson.VideoURL.Replace("/watch?v=", "/embed/");
                    }
                }

                if (pdfUpload != null)
                {
                    string filename = pdfUpload.FileName;

                    string ext = filename.Substring(filename.LastIndexOf("."));

                    string[] goodExts = { ".pdf" };

                    if (goodExts.Contains(ext))
                    {
                        pdfUpload.SaveAs(Server.MapPath("~/Content/resources/" + filename));
                        lesson.PdfFilename = filename;
                    }
                    else
                    {
                        ViewBag.CourseId = new SelectList(db.Courses, "CourseID", "CourseName", lesson.CourseId);
                        ViewBag.ErrorMessage = "* Only PDF files are allowed";
                        return View(lesson);
                    }

                }

                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "HRAdmin, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [Authorize(Roles = "HRAdmin, Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
            db.SaveChanges();

            return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
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
