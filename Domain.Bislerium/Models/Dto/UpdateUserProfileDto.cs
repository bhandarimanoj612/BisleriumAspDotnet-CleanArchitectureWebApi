using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models.Dto
{
    public class UpdateUserProfileDto
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        //public string? ProfileImg { get; set; } = null; // for profile image for user
    }
}
