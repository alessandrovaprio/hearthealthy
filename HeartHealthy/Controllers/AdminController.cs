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
using System.Configuration;

namespace HeartHealthy.Controllers
{
    public class AdminController : Controller
    {
        public string connString= "Data Source=HeartHealthy.db;";
        public IActionResult Index()
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                AdminView modello = new AdminView();

                var users = db.GetUsers;
                var fitbits = db.GetFitbits;

                modello.Admin = 0;
                modello.Dottori = 0;
                modello.Fitbit = 0;
                modello.FitbitNoReg = 0;

                foreach(var fitbit in fitbits){
                    if (fitbit.DoctorId != null)
                        modello.Fitbit++;
                    else
                        modello.FitbitNoReg++;
                }
                if (users == null)
                {
                    modello.Utenti = 0;

                }
                else
                {
                    foreach (var User in users)
                    {
                        modello.Utenti++;
                        if (User.Doctor)
                            modello.Dottori++;
                        else
                            modello.Admin++;
                    }

                }
                return View(modello);
            }
        }
        public IActionResult Profilo(int id)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                ProfiloView modello = new ProfiloView();

                var user = db.GetUsers.Where(u => u.Id == id).FirstOrDefault();

                if (user == null)
                {
                    return View("Profilo", null);
                }
                modello.Id = user.Id;
                modello.Nome = user.Nome;
                modello.Cognome = user.Cognome;
                modello.Email = user.Email;
                modello.Password = user.Password;
                if (user.Doctor == true)
                    modello.Ruolo = "Doctor";
                else
                    modello.Ruolo = "Admin";
                return View("Profilo", modello);
            }
        }
        public IActionResult UpdateProfilo(int id)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                ProfiloView modello = new ProfiloView();

                var user = db.GetUsers.Where(u => u.Id == id);

                if (user == null)
                {
                    return View("UpdateProfilo", null);
                }

                var User = user.ElementAt(0);

                modello.Id = User.Id;
                modello.Nome = User.Nome;
                modello.Cognome = User.Cognome;
                modello.Email = User.Email;
                modello.Password = User.Password;
                modello.Ruolo = User.Doctor == true ? "Doctor" : "Admin";

                return View("UpdateProfilo", modello);
            }
        }

        public IActionResult ElencoUser(int id,bool all,bool doctor)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                List<UsersView> modello = new List<UsersView>();

                var users = db.GetUsers.Where(u => u.Id != id);
                ViewData["userstype"] = "Utenti";
                if (!all){
                    users = db.GetUsers.Where(u => u.Id != id && u.Doctor == doctor);
                    if(doctor)
                        ViewData["userstype"] = "Dottori";
                    else
                        ViewData["userstype"] = "Amministratori";
                }
                    


                if (users == null)
                {
                    modello = null;
                    return View("ElencoUtenti", modello);
                }
                else
                {


                    foreach (var User in users)
                    {
                        UsersView temp = new UsersView
                        {
                            Id = User.Id,
                            Nome = User.Nome,
                            Cognome = User.Cognome,
                            Email = User.Email,
                            Doctor = User.Doctor
                        };
                        modello.Add(temp);
                    }
                    var prova = JsonConvert.SerializeObject(modello);
                    return View("ElencoUtenti", modello);
                }

            }
        }

        public IActionResult ElencoFitBitAssociati()
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                List<FitbitView> modello = new List<FitbitView>();

                var fitbit = db.GetFitbits;

                if (fitbit == null)
                {
                    modello = null;
                    return View("ElencoFitBitAssociati", modello);
                }
                else
                {
                    var dottori = db.GetUsers.Where(u => u.Doctor == true);

                    foreach (var fit in fitbit)
                    {
                        foreach (var dott in dottori)
                        {

                            if (dott.Id == fit.DoctorId)
                            {
                                FitbitView temp = new FitbitView
                                {
                                    Id = fit.Id,
                                    Nome = fit.Nome,
                                    Cognome = fit.Cognome,
                                    Mail = fit.Mail,
                                    NomeDottore = dott.Nome,
                                    CognomeDottore = dott.Cognome
                                };
                                modello.Add(temp);
                                break;
                            }
                        }
                    }
                    var prova = JsonConvert.SerializeObject(modello);
                    return View("ElencoFitBitAssociati", modello);
                }

            }
        }

        public IActionResult Update(int id, string pass, string ruolo,Users modello)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {

                modello.Id = id;
                modello.Password = pass;
                modello.Doctor = ruolo.ToLower() == "doctor" ? true : false;
                db.Update(modello);
                return RedirectToAction("Profilo", "Admin", new { id = modello.Id });
            }
        }

        public IActionResult UpdatePassword(string pass)
        {
            Users user = new Users();
            user.Password = pass;
            return View("UpdatePassword", user);
        }

        public IActionResult ChangePassword(int id, Users modello)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var user = db.GetUsers.Where(u => u.Id == id);

                var User = user.ElementAt(0);

                modello.FP = User.FP;
                modello.Cognome = User.Cognome;
                modello.Nome = User.Nome;
                modello.Email = User.Email;
                modello.Doctor = User.Doctor;
                modello.Id = id;
                db.Update(modello);
                return RedirectToAction("Profilo", "Admin", new { id = modello.Id });
            }
        }
        public IActionResult UpdateRuolo(int id,int idAdmin)
        {
             using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var user = db.GetUsers.Where(u => u.Id == id).FirstOrDefault();
                Users modello = new Users();

                modello.FP = user.FP;
                modello.Cognome = user.Cognome;
                modello.Nome = user.Nome;
                modello.Email = user.Email;
                modello.Password = user.Password;
                modello.Id = id;

                if (user.Doctor == true)
                    modello.Doctor = false;
                else
                    modello.Doctor = true;

                db.Update(modello);

                var fitbit = db.GetFitbits.Where(u => u.DoctorId == id);
                if(fitbit != null)
                {
                    foreach (var fit in fitbit)
                    {
                        Fitbit temp = new Fitbit()
                        {
                            Id = fit.Id,
                            Nome = fit.Nome,
                            Cognome = fit.Cognome,
                            Mail = fit.Mail,
                            GirnoAssociazione = null,
                            DoctorId = null
                        };

                        db.Update(temp);
                    }
                }
                return RedirectToAction("ElencoUser", "Admin",new { id = idAdmin });
            }
        }

        public IActionResult ElencoFitBitNonAssociati()
        {  
                    using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                List<FitbitView> modello = new List<FitbitView>();

                var fitbit = db.GetFitbits.Where(u => u.DoctorId == null);

                if (fitbit == null)
                {
                    modello = null;
                    return View("ElencoFitBitNonAssociati", modello);
                }
                else
                {
                    foreach (var fit in fitbit)
                    {
                        FitbitView temp = new FitbitView
                        {
                            Id = fit.Id,
                            Nome = fit.Nome,
                            Cognome = fit.Cognome,
                            Mail = fit.Mail,
                        };
                        modello.Add(temp);
                    }
                    return View("ElencoFitBitNonAssociati", modello);
                }

            }
        }
        public IActionResult AssociaMedico(int id,int idmedico)
        {
                    using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {

                var fitbit = db.GetFitbits.Where(u => u.Id == id).FirstOrDefault();

                Fitbit fit = new Fitbit();

                fit.Id = id;
                fit.Cognome = fitbit.Cognome;
                fit.Nome = fitbit.Nome;
                fit.DoctorId = idmedico;
                fit.Mail = fitbit.Mail;
                fit.GirnoAssociazione = DateTime.Today;

                db.Update(fit);

                return RedirectToAction("ElencoFitBitNonAssociati", "Admin");
            }
        }
        public IActionResult Associa(int id)
        {
                    using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                List<AssociaView> modello = new List<AssociaView>();

                var users = db.GetUsers.Where(u => u.Doctor == true);

                if (users.Count() == 0)
                {
                    return RedirectToAction("ElencoFitBitNonAssociati", "Admin");
                }
                else
                {
                    foreach (var User in users)
                    {
                        AssociaView temp = new AssociaView
                        {
                            DoctorId = User.Id,
                            FitBitId = id,
                            NomeDoctor = User.Nome,
                            CognomeDoctor = User.Cognome,
                        };
                        modello.Add(temp);
                    }
                    var prova = JsonConvert.SerializeObject(modello);
                    return View("Associa", modello);
                }
            }
        }

        public IActionResult DeassociaMedico(int id)
        {
                    using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                Fitbit fit = new Fitbit();

                var fitbit = db.GetFitbits.Where(u => u.Id == id).FirstOrDefault();

                fit.Id = id;
                fit.DoctorId = null;
                fit.GirnoAssociazione = null;
                fit.Mail = fitbit.Mail;
                fit.Nome = fitbit.Nome;
                fit.Cognome = fitbit.Cognome;

                db.Update(fit);

                return RedirectToAction("ElencoFitBitAssociati", "Admin");

            }
        }
    }

}