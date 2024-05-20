using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models.Dto
{
    public class UserInfoResult
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProfileImg { get; set; } // Profile image URL
        public IEnumerable<string> Roles { get; set; }
    }
}
