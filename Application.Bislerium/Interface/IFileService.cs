using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface IFileService
    {
        //for file image upload
        Task<string> WriteFile(IFormFile file);
    }
}
