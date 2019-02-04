using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace HeartHealthy.Models
{
    public class Percorso
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        [NotNull]
        public DateTime DataRegistrazione { get; set; }
        [NotNull]
        public int IdFitBit { get; set; }
        [NotNull]
        public int Km { get; set; }
        [NotNull]
        public int Passi { get; set; }
        [NotNull]
        public int MSalita { get; set; }
        [NotNull]
        public int MDiscesa { get; set; }
        [NotNull]
        public float VelocitaMedia { get; set; }
        [NotNull]
        public string OrarioInizio { get; set; }
        [NotNull]
        public string OrarioFine { get; set; }
    }
}
