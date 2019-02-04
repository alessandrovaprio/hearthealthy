using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace HeartHealthy.Models
{
    public class Allenamento
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        [NotNull]
        public int IdFitBit { get; set; }
        [NotNull]
        public int Durata { get; set; }
        [NotNull]
        public int Frequenza { get; set; }
        [NotNull]
        public int PesoTarget { get; set; }
        [NotNull]
        public DateTime DataAssegnamento { get; set; }
    }
}
