using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    public class CommentHistory
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string Content { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; } = null;
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
    }
}
