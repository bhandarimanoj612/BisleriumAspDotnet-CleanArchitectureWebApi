using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models.Dto
{
    public class VerifyOTPDTO
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
