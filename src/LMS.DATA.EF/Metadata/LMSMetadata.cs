﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.DATA.EF
{
    [MetadataType(typeof(CoursMetadata))]
    public partial class Course{}

    public class CoursMetadata
    {
        [Display(Name = "Course Name")]
        [StringLength(200, ErrorMessage = "* Maximum of 200 characters")]
        [Required(ErrorMessage = "* Course name is required")]
        public string CourseName { get; set; }


        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "* Maximum of 500 characters")]
        public string CourseDescription { get; set; }

        [Display(Name = "Tag")]
        [StringLength(100, ErrorMessage = "* Maximum of 500 characters")]
        public string CourseTag { get; set; }

        [Display(Name = "Course Image")]
        [StringLength(100, ErrorMessage = "* Maximum of 200 characters")]
        public string CourseImage { get; set; }

        [Display(Name = "Course Time")]
        public int CourseTime { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }


    [MetadataType(typeof(CourseCompletionMetadata))]
    public partial class CourseCompletion { }

    public class CourseCompletionMetadata
    {
        [Display(Name = "Date Completed")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime DateCompleted { get; set; }
    }



    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson { }

    public class LessonMetadata
    {
        [Display(Name = "Lesson")]
        [Required(ErrorMessage = "* Lesson name is required")]
        [StringLength(200, ErrorMessage = "* Maximum of 200 characters")]
        public string LessonTitle { get; set; }

        [Required(ErrorMessage = "* Introduction is required")]
        [StringLength(300, ErrorMessage = "* Maximum of 300 characters")]
        public string Introduction { get; set; }

        [Display(Name = "Video URL")]
        [StringLength(250, ErrorMessage = "* Maximum of 250 characters")]
        public string VideoURL { get; set; }

        [Display(Name = "PDF Resource")]
        [StringLength(100, ErrorMessage = "* Maximum of 100 characters")]
        public string PdfFilename { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }



    [MetadataType(typeof(LessonViewMetadata))]
    public partial class LessonView { }

    public class LessonViewMetadata
    {

        [Display(Name = "Date Viewed")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public System.DateTime DateViewed { get; set; }

        public virtual Lesson Lesson { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }



    [MetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail { }

    public class UserDetailMetadata
    {

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "* First Name is required")]
        [StringLength(50, ErrorMessage = "* Max length 50 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "* Last Name is required")]
        [StringLength(50, ErrorMessage = "* Max length 50 characters")]
        public string LastName { get; set; }
    }

}
