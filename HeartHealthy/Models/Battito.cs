using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class Battito
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        [NotNull]
        public int IdFitBit { get; set; }
        [NotNull]
        public int Frequenza { get; set; }
        [NotNull]
        public DateTime DataRegistrazione { get; set; }
    }
}
