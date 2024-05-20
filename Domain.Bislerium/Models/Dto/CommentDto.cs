using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models.Dto
{
    public class CommentDto
    { 
        public string Content { get; set; }

        public string UserName { get; set; }
        public string UserProfile { get; set; }
    }
}
