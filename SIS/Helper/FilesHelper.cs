using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SIS.Helper
{
    public class FilesHelper
    {
        private readonly IWebHostEnvironment webHost;

        public FilesHelper(IWebHostEnvironment webHost)
        {
            this.webHost = webHost;
        }
        public FilesHelper()
        {
                
        }

        public string UplodeFile(IFormFile file , string folder)
        {
            if (file != null)
            {
                
                var fileName = Guid.NewGuid() + "_" + file.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (FileStream fileStream = new FileStream(filePath , FileMode.Create))
                {
                    file.CopyTo(fileStream);
                    return filePath;
                }
            }
            else
            {
                return String.Empty;
            }
        }
        public string UplodeFile2(IFormFile file, string folder)
        {
            if (file != null)
            {

                var fileName = Guid.NewGuid() + "_" + file.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder, fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                    return fileName;
                }
            }
            else
            {
                return String.Empty;
            }
        }

    }
}
