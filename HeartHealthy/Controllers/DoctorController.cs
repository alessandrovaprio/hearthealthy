using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.DataProvider.SQLite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HeartHealthy.Data;
using HeartHealthy.Models;

namespace HeartHealthy.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        public string connString = "Data Source=HeartHealthy.db;";
        public IActionResult Index()
        {
            var userMail = HttpContext.User.Claims.ToList().ElementAt(2).Value;

            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {

                var users = db.GetUsers.Where(u => u.Email == userMail).FirstOrDefault();
                var pazienti = db.GetFitbits.Where(p => p.DoctorId == users.Id);
                List<Fitbit> fitbits = pazienti.ToList();
                var allenamenti = from f in db.GetFitbits
                                  join a in db.GetAllenamento on f.Id equals a.IdFitBit
                                  where f.DoctorId == users.Id
                                  select a;
                var battiti = db.GetBattito;

                DoctorModels models = new DoctorModels
                {
                    fitbits = fitbits,
                    Allenamenti = allenamenti.ToList(),
                };
                models.AllenamentiTot = allenamenti.ToList().Count();
                if (battiti.ToList().Count == 0)
                {
                    models.Battiti = 0;
                }
                else
                {
                    foreach (var bat in battiti.ToList())
                    {
                        TimeSpan span = DateTime.Today.Subtract(bat.DataRegistrazione);
                        if (span.Days < 1)
                            models.Battiti++;
                    }
                }
                if (models.Allenamenti.Count == 0)
                    models.AllenmentiSettimana = 0;
                else
                {

                    foreach (var allenamento in models.Allenamenti)
                    {
                        TimeSpan span = DateTime.Today.Subtract(allenamento.DataAssegnamento);
                        if (span.Days < 7)
                            models.AllenmentiSettimana++;
                    }
                }

                return View(models);
            }
        }
        /// <summary>
        /// Action per il monitoraggio vengono settati i parametri per la selezione di un paziente o di tutti i pazienti assegnati al dottore
        /// loggato, più il relativo range di date.
        /// </summary>
        /// <returns></returns>
        public ActionResult MonitoraggioToView()
        {
            var userMail = HttpContext.User.Claims.ToList().ElementAt(2).Value;
            List<Fitbit> fitbits = null;
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var users = db.GetUsers.Where(u => u.Email == userMail).FirstOrDefault();
                var pazienti = db.GetFitbits.Where(p => p.DoctorId == users.Id);
                fitbits = pazienti.ToList();
            }
            return View("MonitoraggioToView", fitbits);
        }
        /// <summary>
        /// viene creato l'oggetto contente i o il paziente selezionato con i relativi dati sui battiti km percorsi 
        /// e renderizzzata la vista contente tale informazione
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult Monitoraggio(DataMonitoraggio data)
        {
            var userMail = HttpContext.User.Claims.ToList().ElementAt(2).Value;

            Fitbit paziente = null;
            List<Battito> battiti = new List<Battito>();
            List<Percorso> percorsi = new List<Percorso>();
            List<Peso> peso = new List<Peso>();
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var users = db.GetUsers.Where(u => u.Email == userMail).FirstOrDefault();
                paziente = db.GetFitbits.Where(p => p.Id == data.Paziente).FirstOrDefault();
                battiti = db.GetBattito.Where(b => b.IdFitBit == paziente.Id && b.DataRegistrazione >= data.DataInizio && b.DataRegistrazione <= data.DataFine).ToList();
                percorsi = db.GetPercorso.Where(p => p.IdFitBit == paziente.Id && p.DataRegistrazione >= data.DataInizio && p.DataRegistrazione <= data.DataFine).ToList();
                peso = db.GetPeso.Where(p => p.IdFitBit == paziente.Id && p.DataRegistrazione >= data.DataInizio && p.DataRegistrazione <= data.DataFine).ToList();
            }

            MonitoraggioModels models = new MonitoraggioModels
            {
                DataInizio = data.DataInizio,
                DataFine = data.DataFine,
                BattitiperPaziente = battiti,
                Peso = peso,
                Percorsi = percorsi,
                CoordBattiti = "",
                CoordPercorso = "",
                DateCoordBattiti = "",
                DateCoordPercorso = "",
                CoordPassi = "",
                CoorddurataAttività = "",
                DateCoordPeso = "",
                CoordPeso = ""
            };
            //creo le stringhe per il grafico dei battiti
            foreach (var bat in battiti)
            {
                if (models.CoordBattiti == "")
                {
                    models.DateCoordBattiti = bat.DataRegistrazione.ToShortDateString();
                    models.CoordBattiti = bat.Frequenza.ToString();

                }
                else
                {
                    models.CoordBattiti = models.CoordBattiti + " " + bat.Frequenza;
                    models.DateCoordBattiti = models.DateCoordBattiti + "," + bat.DataRegistrazione.ToShortDateString();
                }
            };
            //creo le stringhe per il grafico dei km percorsi, passi e durata attività
            foreach (var per in percorsi)
            {
                DateTime inizio = DateTime.Parse(per.OrarioInizio);
                DateTime fine = DateTime.Parse(per.OrarioFine);
                var durata = (fine - inizio).TotalMinutes;
                if (models.CoordPercorso == "")
                {
                    models.DateCoordPercorso = per.DataRegistrazione.ToShortDateString();
                    models.CoordPercorso = per.Km.ToString();
                    models.CoordPassi = per.Passi.ToString();
                    models.CoorddurataAttività = durata.ToString();
                }
                else
                {
                    models.CoordPercorso = models.CoordPercorso + " " + per.Km;
                    models.DateCoordPercorso = models.DateCoordPercorso + "," + per.DataRegistrazione.ToShortDateString();
                    models.CoordPassi = models.CoordPassi + " " + per.Passi;
                    models.CoorddurataAttività = models.CoorddurataAttività + " " + durata;
                }
            };
            //creo le stringhe per i grafici del peso
            foreach (var pe in peso)
            {
                if (models.CoordBattiti == "")
                {
                    models.DateCoordPeso = pe.DataRegistrazione.ToShortDateString();
                    models.CoordPeso = pe.PesoIstantaneo.ToString();

                }
                else
                {
                    models.CoordPeso = models.CoordPeso + " " + pe.PesoIstantaneo;
                    models.DateCoordPeso = models.DateCoordPeso + "," + pe.DataRegistrazione.ToShortDateString();
                }
            };
            models.Paziente = paziente;

            return View("Monitoraggio", models);
        }
        //gestione degli allenamenti
        public ActionResult AllenamentoToView()
        {
            var userMail = HttpContext.User.Claims.ToList().ElementAt(2).Value;
            List<Fitbit> fitbits = null;
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var users = db.GetUsers.Where(u => u.Email == userMail).FirstOrDefault();
                var pazienti = db.GetFitbits.Where(p => p.DoctorId == users.Id);
                fitbits = pazienti.ToList();
            }
            return View("AllenamentoToView", fitbits);
        }
        public ActionResult Allenamento(int FitBit)
        {
            if(FitBit == 0)
                return RedirectToAction("Index", "Doctor");

            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var allenamenti = db.GetAllenamento.Where(a => a.IdFitBit == FitBit).ToList();
                List<Attivita> attività = new List<Attivita>();
                foreach (var all in allenamenti)
                {
                    if (db.GetAttivita.Where(a => a.IdAllenamento == all.Id) != null)
                        attività.AddRange(db.GetAttivita.Where(a => a.IdAllenamento == all.Id).ToList());
                }
                AllenamentoModel model = new AllenamentoModel();
                model.Allenamento = allenamenti;
                model.Attivita = attività;
                return View("Allenamento", model);
            }

        }
        //insermento allenammento
        public ActionResult InsertAllenamento()
        {
            var userMail = HttpContext.User.Claims.ToList().ElementAt(2).Value;
            List<Fitbit> fitbits = null;
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var users = db.GetUsers.Where(u => u.Email == userMail).FirstOrDefault();
                var pazienti = db.GetFitbits.Where(p => p.DoctorId == users.Id);
                fitbits = pazienti.ToList();
            }
            return View("InsertAllenamento", fitbits);
        }
        //inserisco l'allenamento e creo le nuove attività
        public ActionResult Aggiungi(Allenamento allenamento)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                db.Insert(allenamento);
            }
            return View("InsertAttivitàN");
        }
        //inserisco le nuove attività
        public ActionResult InsertAttivitàN(Attivita attivita)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var allenamento = db.GetAllenamento.ToList().Last();
                attivita.IdAllenamento = allenamento.Id;
                db.Insert(attivita);
                if (attivita.AltreAttivita == null)
                    return Allenamento(allenamento.IdFitBit);
                else
                    return View("InsertAttivitàN");
            }
        }
        //eliminazione allenamenti

        public ActionResult EliminaAllenamento(int id)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                int idfitbit = db.GetAllenamento.Where(i => i.Id == id).FirstOrDefault().IdFitBit;
                db.Delete(db.GetAllenamento.Where(i => i.Id == id).First());
                var attività = db.GetAttivita.Where(a => a.IdAllenamento == id).ToList();
                if (attività != null)
                    foreach (var att in attività)
                    {
                        db.Delete(att);
                    }
                return Allenamento(idfitbit);
            }
        }
        //gestione della mdifica degli allenamenti
        public ActionResult ModificaAllenamento(int id)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var allenamento = db.GetAllenamento.Where(i => i.Id == id).FirstOrDefault();
                return View("ModAll", allenamento);
            }
        }
        public ActionResult ModAll(Allenamento model)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                db.Update(model);
                return Allenamento(model.IdFitBit);
            }
        }
        //eliminazione attività
        public ActionResult EliminaAttivita(int id)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                int idfitbit = db.GetAllenamento.Where(a => a.Id == db.GetAttivita.Where(i => i.Id == id).FirstOrDefault().IdAllenamento).FirstOrDefault().IdFitBit;
                db.Delete(db.GetAttivita.Where(i => i.Id == id).First());
                return Allenamento(idfitbit);
            }
        }
        //gestione della mdifica delle attività
        public ActionResult ModificaAttività(int id)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var att = db.GetAttivita.Where(i => i.Id == id).FirstOrDefault();
                return View("ModAtt", att);
            }
        }
        public ActionResult ModAtt(Attivita model)
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                int idfitbit = db.GetAllenamento.Where(a => a.Id == db.GetAttivita.Where(i => i.Id == model.Id).FirstOrDefault().IdAllenamento).FirstOrDefault().IdFitBit;
                db.Update(model);
                return Allenamento(idfitbit);
            }
        }
        //Aggiunta nuove Attività
        public ActionResult InsertAttivita()
        {
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                var userMail = HttpContext.User.Claims.ToList().ElementAt(2).Value;
                var users = db.GetUsers.Where(u => u.Email == userMail).FirstOrDefault();
                List<EliminaModel> model = db.GetFitbits.Where(p => p.DoctorId == users.Id).ToList().Join(db.GetAllenamento, f => f.Id, a => a.IdFitBit, (f, a) => new EliminaModel { Id = a.Id, Mail = f.Mail }).ToList();
                return View("InsertAttività", model);
            }

        }
        public ActionResult AggiungiAttivita(Attivita att)
        {
            if(att.IdAllenamento == 0)
                return RedirectToAction("Index", "Doctor");
            using (var db = new HealthDataContext(SQLiteTools.GetDataProvider(), connString))
            {
                db.Insert(att);
                int idfitbit = db.GetAllenamento.Where(a => a.Id == db.GetAttivita.ToList().Last().IdAllenamento).FirstOrDefault().IdFitBit;
                return Allenamento(idfitbit);
            }
        }
    }
}
