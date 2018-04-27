using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using StoreBox.Models;

namespace StoreBox.Controllers
{
    public class HomeController : Controller
    {

        private FileContext _context;
        private List<FileInfo> Files;
        private List<DirectoryInfo> Directories;
        private string user;

        public HomeController(FileContext context){

            _context = context;
            Files = new List<FileInfo>();
            Directories = new List<DirectoryInfo>();


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Index()
        {

            this.user = GetUserFromToken();
            var currentDirectory = Request.Path.Value;
            var length = currentDirectory.Length;
            int index = currentDirectory.LastIndexOf("/");
            if (index > 0)
                currentDirectory = currentDirectory.Substring(index, length-index );
            if(currentDirectory != "/home"){

                this.user = this.user + currentDirectory;
            }
            GetEnvironnementFilesInfos(user);
            GetEnvironnementDirectoryInfos(user);
            ViewData["files"] = this.Files;
            ViewData["directories"] = this.Directories;
              
                return View(); 
        



        }

        public void GetEnvironnementDirectoryInfos(String user){

            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", user);
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));

            foreach (string directoryPath in dirs)
            {


                this.Directories.Add(new DirectoryInfo(directoryPath));

            }


        }

         
        public void GetEnvironnementFilesInfos(String user){

            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", user);

            List<string> dirs = new List<string>(Directory.EnumerateFiles(dirPath));

            foreach (string filePath in dirs){

            
                this.Files.Add(new FileInfo(filePath));
                
            }


        } 

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> UploadFile(IFormFile file)  
        {  
            this.user = GetUserFromToken();
            var chemin = HttpContext.Request.Form["chemin"];
            var currentDirectory = Request.Path.Value;
            var length = currentDirectory.Length;
            int index = currentDirectory.LastIndexOf("/");
            if (index > 0)
                currentDirectory = currentDirectory.Substring(index, length - index);
            if (currentDirectory != "/home")
            {

                this.user = this.user + currentDirectory;
            }
            if (file == null || file.Length == 0)  
                return Content("file not selected");
            
            var path = Path.Combine(  
                                    Directory.GetCurrentDirectory(), "Data", GetUserFromToken() , file.FileName);
            
  
            using (var stream = new FileStream(path, FileMode.Create))  
            {  
                await file.CopyToAsync(stream);  
            }  
  
            return RedirectToAction("Index");  
        }

        public async Task<IActionResult> Download()  
  {


            //if (filename == null)  
            //return Content("filename not present");  

            var path = HttpContext.Request.Form["path"];
            Console.WriteLine("TESTTTTTT" + path);
  
      var memory = new MemoryStream();  
      using (var stream = new FileStream(path, FileMode.Open))  
      {  
          await stream.CopyToAsync(memory);  
      }  
      memory.Position = 0;  
            return File(memory, GetContentType(path), Path.GetFileName(path));  
  }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        private string GetUserFromToken()
        { //recupère le user correspondant à l'user

            string v = this.HttpContext.Request.Headers["Authorization"];
            string token = v.Substring("Bearer ".Length).Trim();
            var sp = new JwtSecurityToken(token);
            var claims = sp.Claims;
            var t = claims.ToList();
            return t[0].Value;
        }

    }
}
