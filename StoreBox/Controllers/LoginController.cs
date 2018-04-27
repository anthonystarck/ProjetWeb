using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StoreBox.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreBox.Controllers
{
    public class LoginController : Controller
    {
        private FileContext _context;

        public LoginController(FileContext context)
        {
            _context = context; //base de données
        }

        public IActionResult Auth(){

            return View("~/Views/Login/Login.cshtml");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login() //fonction de login
        {

            string username = HttpContext.Request.Form["username"];
            string password = HttpContext.Request.Form["password"];
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); //si le l'état du modele n'est pas valide retourne un badrequest
            }

            User user;
            Regex regMail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"); //regex du mail
            Match m = regMail.Match(username);
            if (m.Success) //si le regex est bon
            {
                user = await _context.User.SingleOrDefaultAsync(u => u.mail == username); //récupération de l'user correspondant à l'email
            }
            else
            {
                user = await _context.User.SingleOrDefaultAsync(u => u.username == username); //récupération de l'user correspondant à l'username
            }

            if (user == null)
            {
                return BadRequest(new { message = "Login inccorect" }); //si aucun utilisateur n'est trouvé alors badrequest 
            }

            var result = await Task.Run(() => CheckPasswordSignInAsync(user, password)); //vérifie si le mot de passe est correcte
            if (result)
            {

                var claims = new[]
                {
                      new Claim(JwtRegisteredClaimNames.UniqueName, user.username), //ajoute l'username correspondant au token
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("FQ=LTPg6Bxs8.A:}i&do:^!P")); //applique la clé de validation au token
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken("http://localhost/home",
                                                 "http://localhost/home",
                  claims,
                  expires: DateTime.Now.AddMinutes(30), //temps avant expiration du token
                  signingCredentials: creds);
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);




                HttpContext.Response.Cookies.Append(
            "access-token",
                    encodedJwt,
            new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Path = "/"
            }
        );


                return RedirectToAction(nameof(HomeController.Index), "Home") ;
                 // crée et retourne le token
            }
            else
            {
                return BadRequest(new { message = "Mot de passe incorect" }); // si le mot de passe est incorrect retourne badrequest
            }
        }



        private bool CheckPasswordSignInAsync(User user, string password)
        {
            var hash = GetHashed(password, user.salt);
            Console.WriteLine("PASSSSSSSSSSSSSSSSS : " + hash);
            if (GetHashed(password, user.salt) == user.password){ // si le mot de passe entré hashé correspond au mot de passe hashé de la bdd
                
                return true;
        }
            return false;
        }

        private string GetHashed(string password, string salt)
        {
            string toHash = password + salt; //ajoute le sel au mot de passe 
            SHA512 shaM = new SHA512Managed();
            return Convert.ToBase64String(shaM.ComputeHash(Encoding.ASCII.GetBytes(toHash))); // Hash le mdp et le retourne en base64
        }



    }
}
  
