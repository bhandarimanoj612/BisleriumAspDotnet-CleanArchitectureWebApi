using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    public class BlogPostHistory
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? UpdatedByUserId { get; set; } = null;
        public string? UpdatedByUserName { get; set; } = null;
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string ImageUrl { get; set; }
        public string Action { get; set; }//is created or delted or updated
    }
}
