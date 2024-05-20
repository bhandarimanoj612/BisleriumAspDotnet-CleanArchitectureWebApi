using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Bislerium.Models
{
    public class BlogPost
    {

        //blog post id 
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
      //blog post image will saved here
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfile { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public string? DeletedByUserId { get; set; } = null;

        public int TotalReaction { get; set; }

        public int TotalPopularity { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<BlogReaction> BlogReactions { get; set; }
    }
}
