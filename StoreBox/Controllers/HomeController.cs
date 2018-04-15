using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreBox.Models;

namespace StoreBox.Controllers
{
    public class HomeController : Controller
    {

        private FileContext _context;

        public HomeController(FileContext context){

            _context = context;
        }

        public IActionResult Index()
        {

            var user = "antho"; //récupère l'utilisateur associé au token utilisé 
            var user_id = _context.User.FirstOrDefault(t => t.username == user);//récupère l'id de l'utilisateur correspondant
            Console.WriteLine(user_id.username);
            var result = _context.File.Where(u => u.idUser == user_id.ID).ToList(); //récupère les todo correspondant à l'user 
            //if (result.LongCount() == 0)
            //{ //s'il ne trouve pas de file

             //   return Ok("Pas de fichiers pour cet utilisateur"); //retourne un ok 
            //}
            //else
            //{
                ViewData["files"] = result; 
              
                return View(); //retourne un json contenant les infos sur les todo utilisateurs
            //}



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
    }
}
