using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{//for sending iemail services
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
