using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace HeartHealthy.Models
{
    public class Peso
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        [NotNull]
        public int IdFitBit { get; set; }
        [NotNull]
        public int PesoIstantaneo { get; set; }
        [NotNull]
        public DateTime DataRegistrazione { get; set; }
    }
}
