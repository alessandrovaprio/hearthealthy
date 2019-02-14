using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.DataProvider.SQLite;
using Microsoft.AspNetCore.Mvc;
using HeartHealthy.Data;
using HeartHealthy.Models;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Threading;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;

namespace HeartHealthy.Controllers
{
    
    public class LoginController : Controller
    {
        public string connString = "Data Source=HeartHealthy.db;";
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginView model)
        {
                ///Validate user
           using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
           {

                var users = db.GetUsers.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();
                if (users == null)
                {
                    ModelState.AddModelError("", "Email o Password errate ");
                    return View("Index");
                }
                else
                  if (users.FP == true)
                    return View("ChangePassword");
                else
                {
                    // create claims
                    List<Claim> claims = new List<Claim>
                     {
                         new Claim(ClaimTypes.SerialNumber, users.Id.ToString()),
                         new Claim(ClaimTypes.Name, users.Nome),
                         new Claim(ClaimTypes.Email, users.Email)
                    };

                    // create identity
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // create principal
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    // sign-in
                    await HttpContext.SignInAsync(
                            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                            principal: principal);
                    if (users.Doctor)
                    {
                        return RedirectToAction("Index", "Doctor");
                    }
                    else
                      return RedirectToAction("Index","Admin");
                }
           }
            
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                            scheme: CookieAuthenticationDefaults.AuthenticationScheme
                            );
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ChangePassword (string oldPsw,string newPsw)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {

                var users = db.GetUsers.Where(u => u.Password == oldPsw).FirstOrDefault();
                if (users == null)
                {
                    ModelState.AddModelError("", "Utente non trovato ");
                    return View("ChangePassword");
                }
                users.Password = newPsw;
                users.FP = false;
                db.Update(users, "Users");
                LoginView model = new LoginView
                {
                    Email = users.Email,
                    Password = users.Password
                };
                List<Claim> claims = new List<Claim>
                     {
                         new Claim(ClaimTypes.SerialNumber, users.Id.ToString()),
                         new Claim(ClaimTypes.Name, users.Nome),
                         new Claim(ClaimTypes.Email, users.Email)
                    };

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // sign-in
                await HttpContext.SignInAsync(
                        scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: principal);

                if (users.Doctor)
                {
                    return RedirectToAction("Index", "Doctor");
                }
                else
                    return RedirectToAction("Index", "Admin");
                //return RedirectToAction("Index","Login");
            }
        }
        public  ActionResult ResgistratiToView()
        {
            return View("Registrati");
        }
        public ActionResult Registrati (Users model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
                {
                    var users = db.GetUsers.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();
                    if (users == null)
                        db.Insert(new Users { Nome = model.Nome, Email = model.Email, Password = model.Password, Cognome = model.Cognome, FP=false,Doctor=true });
                    else
                    {
                        ModelState.AddModelError("", "Utente già iscritto");
                        return View("Index");
                    }
                }
            }
            return View();
        }
        public ActionResult ForgotToView ()
        {
            return View("Forgot");
        }
        public ActionResult Forgot (string mail)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var users = db.GetUsers.Where(u => u.Email == mail).FirstOrDefault();
                string randomPassword = GenerateRandomPassword();
                if (users!=null)
                { 
                    users.FP = true;
                    users.Password = randomPassword;
                    db.Update(users,"Users");
                    string to = mail;
                    string from = "cshearthealthy@gmail.com";
                    MailMessage message = new MailMessage(from, to);
                    message.Subject = "Richiesta cambio Password";
                    message.Body = @"Grazie per averci contattato! ti forniamo la tua password temporanea "+randomPassword;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.Credentials= new NetworkCredential("cshearthealthy@gmail.com", "zYbxum-ficqo9-hojguq");
                    client.Send(message);
                    return View("ForgotThanks");
                }
                else
                {
                    ModelState.AddModelError("", "Utente non trovato");
                    return View("Forgot");
                }
            }
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$?_-"                        // non-alphanumeric
    };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
