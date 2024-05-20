using Application.Bislerium.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class FileService : IFileService
    {

        //below is code for file upload of image 
        public async Task<string> WriteFile(IFormFile file)
        {
            string filename = "";
            try
            {
                // Check file size
                if (file.Length > 3 * 1024 * 1024) // 3 MB in bytes
                {
                    throw new Exception("File size exceeds the maximum limit (3MB).");
                }

                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = DateTime.Now.Ticks.ToString() + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\Files");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(filepath, filename);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw; // Rethrow the exception to handle it at a higher level
            }
        }
    }
}
