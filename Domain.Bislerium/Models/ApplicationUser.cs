
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    public class ApplicationUser:IdentityUser
    {
        public object Comments;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]//this will not create any table in database 
        public IList<string> Roles { get; set; }

        public string? VerificationCode { get; set; }

        public string? ProfileImg { get; set; } = null; // for profile image for user
         // Navigation property for BlogPosts
        public ICollection<BlogPost> BlogPosts { get; set; }

    }

}
