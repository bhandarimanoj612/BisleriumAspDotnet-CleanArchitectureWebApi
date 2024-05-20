using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Bislerium.Models
{
    public class CommentReaction
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Comment")]
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public string UserName { get; set; }

        public string Reaction { get; set; } // 'upvote', 'downvote', or 'none'

        public int TotalUpVotes { get; set; } = 0;
        public int TotalDownVotes { get; set; } = 0;
        public int TotalVotes => TotalUpVotes + TotalDownVotes; // Total votes

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public string? DeletedByUserId { get; set; } = null;
    }
}
