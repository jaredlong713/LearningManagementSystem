using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS.UI.MVC.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "* Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Email is required")]
        [EmailAddress(ErrorMessage = "* Please enter a valid email")]
        public string Email { get; set; }

        [UIHint("MultilineText")]
        [Required(ErrorMessage = "* Message is required")]
        public string Message { get; set; }
    }
}