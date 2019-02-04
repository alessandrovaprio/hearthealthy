using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace HeartHealthy.Models
{
    public class Attivita
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        public int IdAllenamento { get; set; }
        [NotNull]
        public int KmMinimi { get; set; }
        [NotNull]
        public int DurataMinima { get; set; }
        [NotNull]
        public int MSalita { get; set; }
        //description conterrà la descrizione dell'attività nel nostro caso corsa o camminata
        [NotNull]
        public string Description { get; set; }
        [NotColumn]
        public bool? AltreAttivita { get; set; }
    }
}
