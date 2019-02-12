using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.DataProvider.SQLite;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HeartHealthy.Data;
using HeartHealthy.Data.Extensions;
using HeartHealthy.Models;

namespace HeartHealthy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

            

            services.AddMvc();
            var dbFactory = new HealthDataContextFactory(
                dataProvider: SQLiteTools.GetDataProvider(),
                connectionString: "Data Source=HeartHealthy.db"
            );
            services.AddSingleton<IDataContextFactory<HealthDataContext>>(dbFactory);
            Prova(dbFactory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });
        }
        public void Prova(IDataContextFactory<HealthDataContext> datacontext)
        {
            using (var db = datacontext.Create())
            {
                db.CreateTableIfNotExists<Users>();
                db.CreateTableIfNotExists<Fitbit>();
                db.CreateTableIfNotExists<Attivita>();
                db.CreateTableIfNotExists<Allenamento>();
                db.CreateTableIfNotExists<Battito>();
                db.CreateTableIfNotExists<Percorso>();
                db.CreateTableIfNotExists<Peso>();

                /*
                db.Insert(new Battito { IdFitBit = 1, DataRegistrazione = DateTime.Today, Frequenza = 66 });
                db.Insert(new Battito { IdFitBit = 1, DataRegistrazione = DateTime.Today, Frequenza = 100 });
                db.Insert(new Battito { IdFitBit = 1, DataRegistrazione = DateTime.Today, Frequenza = 55 });
                db.Insert(new Battito { IdFitBit = 1, DataRegistrazione = DateTime.Today, Frequenza = 88 });
                db.Insert(new Battito { IdFitBit = 1, DataRegistrazione = DateTime.Today, Frequenza = 99 });
                db.Insert(new Battito { IdFitBit = 1, DataRegistrazione = DateTime.Today, Frequenza = 130 });

                db.Insert(new Battito { IdFitBit = 2, DataRegistrazione = DateTime.Today, Frequenza = 55 });
                db.Insert(new Battito { IdFitBit = 2, DataRegistrazione = DateTime.Today, Frequenza = 77 });
                db.Insert(new Battito { IdFitBit = 2, DataRegistrazione = DateTime.Today, Frequenza = 65 });
                db.Insert(new Battito { IdFitBit = 2, DataRegistrazione = DateTime.Today, Frequenza = 98 });
                db.Insert(new Battito { IdFitBit = 2, DataRegistrazione = DateTime.Today, Frequenza = 67 });
                db.Insert(new Battito { IdFitBit = 2, DataRegistrazione = DateTime.Today, Frequenza = 87 });

                db.Insert(new Battito { IdFitBit = 3, DataRegistrazione = DateTime.Today, Frequenza = 55 });
                db.Insert(new Battito { IdFitBit = 3, DataRegistrazione = DateTime.Today, Frequenza = 56 });
                db.Insert(new Battito { IdFitBit = 3, DataRegistrazione = DateTime.Today, Frequenza = 60 });
                db.Insert(new Battito { IdFitBit = 3, DataRegistrazione = DateTime.Today, Frequenza = 76 });
                db.Insert(new Battito { IdFitBit = 3, DataRegistrazione = DateTime.Today, Frequenza = 56 });
                db.Insert(new Battito { IdFitBit = 3, DataRegistrazione = DateTime.Today, Frequenza = 66 });

                db.Insert(new Battito { IdFitBit = 4, DataRegistrazione = DateTime.Today, Frequenza = 77 });
                db.Insert(new Battito { IdFitBit = 4, DataRegistrazione = DateTime.Today, Frequenza = 88 });
                db.Insert(new Battito { IdFitBit = 4, DataRegistrazione = DateTime.Today, Frequenza = 55 });
                db.Insert(new Battito { IdFitBit = 4, DataRegistrazione = DateTime.Today, Frequenza = 140 });
                db.Insert(new Battito { IdFitBit = 4, DataRegistrazione = DateTime.Today, Frequenza = 140 });
                db.Insert(new Battito { IdFitBit = 4, DataRegistrazione = DateTime.Today, Frequenza = 100});

                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 2, MDiscesa = 50, MSalita = 30, Passi = 1000, VelocitaMedia = 8, IdFitBit = 1, OrarioInizio = "18:20", OrarioFine = "18:40" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 5, MDiscesa = 156, MSalita = 156, Passi = 7000, VelocitaMedia = 10, IdFitBit = 1, OrarioInizio = "12:20", OrarioFine = "12:58" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 3, MDiscesa = 200, MSalita = 30, Passi = 4000, VelocitaMedia = 9, IdFitBit = 1, OrarioInizio = "11:50", OrarioFine = "12:50" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 7, MDiscesa = 100, MSalita = 70, Passi = 9000, VelocitaMedia = 7, IdFitBit = 1, OrarioInizio = "11:20", OrarioFine = "12:58" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 2, MDiscesa = 50, MSalita = 150, Passi = 1000, VelocitaMedia = 9, IdFitBit = 1, OrarioInizio = "17:20", OrarioFine = "17:40" });

                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 3, MDiscesa = 50, MSalita = 30, Passi = 1000, VelocitaMedia = 8, IdFitBit = 2, OrarioInizio = "18:20", OrarioFine = "18:40" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 4, MDiscesa = 156, MSalita = 156, Passi = 7000, VelocitaMedia = 10, IdFitBit = 2, OrarioInizio = "12:20", OrarioFine = "12:58" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 5, MDiscesa = 200, MSalita = 30, Passi = 4000, VelocitaMedia = 9, IdFitBit = 2, OrarioInizio = "11:50", OrarioFine = "12:50" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 6, MDiscesa = 100, MSalita = 70, Passi = 9000, VelocitaMedia = 7, IdFitBit = 2, OrarioInizio = "11:20", OrarioFine = "12:58" });
                db.Insert(new Percorso { DataRegistrazione = DateTime.Today, Km = 2, MDiscesa = 50, MSalita = 150, Passi = 1000, VelocitaMedia = 9, IdFitBit = 2, OrarioInizio = "17:20", OrarioFine = "17:40" });

                db.Insert(new Peso { IdFitBit = 1, DataRegistrazione = DateTime.Today, PesoIstantaneo = 80 });
                db.Insert(new Peso { IdFitBit = 1, DataRegistrazione = DateTime.Today, PesoIstantaneo = 81 });
                db.Insert(new Peso { IdFitBit = 1, DataRegistrazione = DateTime.Today, PesoIstantaneo = 82 });
                db.Insert(new Peso { IdFitBit = 1, DataRegistrazione = DateTime.Today, PesoIstantaneo = 81});

                db.Insert(new Peso { IdFitBit = 2, DataRegistrazione = DateTime.Today, PesoIstantaneo = 80 });
                db.Insert(new Peso { IdFitBit = 2, DataRegistrazione = DateTime.Today, PesoIstantaneo = 81 });
                db.Insert(new Peso { IdFitBit = 2, DataRegistrazione = DateTime.Today, PesoIstantaneo = 81 });
                db.Insert(new Peso { IdFitBit = 2, DataRegistrazione = DateTime.Today, PesoIstantaneo = 81 });

                db.Insert(new Peso { IdFitBit = 3, DataRegistrazione = DateTime.Today, PesoIstantaneo = 80 });
                db.Insert(new Peso { IdFitBit = 3, DataRegistrazione = DateTime.Today, PesoIstantaneo = 81 });
                db.Insert(new Peso { IdFitBit = 3, DataRegistrazione = DateTime.Today, PesoIstantaneo = 80 });
                db.Insert(new Peso { IdFitBit = 3, DataRegistrazione = DateTime.Today, PesoIstantaneo = 80 });

                db.Insert(new Fitbit { Nome = "Giacomo", Cognome = "Gallo", Mail = "giacomogallo@gmail.com", DoctorId = null , GirnoAssociazione = null});
                db.Insert(new Fitbit { Nome = "Luca", Cognome = "Abete", Mail = "luca.abete@gmail.com", DoctorId = null, GirnoAssociazione = null });
                db.Insert(new Fitbit { Nome = "Marco", Cognome = "Rossi", Mail = "rossimarco@gmail.com", DoctorId = null, GirnoAssociazione = null });
                db.Insert(new Fitbit { Nome = "Tony", Cognome = "Bianco", Mail = "tony1943@gmail.com", DoctorId = null, GirnoAssociazione = null });

                db.Insert(new Users { Nome = "Paolo",Cognome="Rossi",Email="admin@gmail.com",Password="admin",FP=false,Doctor=false});
                db.Insert(new Users { Nome = "Mario", Cognome = "Rossi", Email = "mario@gmail.com", Password = "rossi", FP = false, Doctor = true });
                db.Insert(new Users { Nome = "Paolo", Cognome = "Verdi", Email = "paolo@gmail.com", Password = "verdi", FP = false, Doctor = true });
                */

            }
        }
    }
}
