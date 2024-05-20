using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("BlogPost")]
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
        public string UserName { get; set; }
        public string UserProfile { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public string? DeletedByUserId { get; set; } = null;

        public int TotalReaction { get; set; }
        public ICollection<CommentReaction> CommentReactions { get; set; }

    }
}
