using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Domain.Bislerium.Models
{
    public class BlogReaction
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BlogPost")]
        public int BlogPostId { get; set; }
        
        public BlogPost BlogPost { get; set; }
        public string UserName { get; set; }

        public string Reaction { get; set; } // 'upvote', 'downvote', or 'none'
        public int TotalUpVotes { get; set; } = 0;//everytime user click upvote show whole user total like and store here for each blog post
        public int TotalDownVotes { get; set; } = 0;//everytime user click upvote show whole user total like and store here for each blog post
        public int TotalVotes => TotalUpVotes + TotalDownVotes; // Total votes

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set;}

         public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public string? DeletedByUserId { get; set; } = null;

       
    }
}
