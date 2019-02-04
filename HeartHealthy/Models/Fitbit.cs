using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class Fitbit
    {
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }
        [Nullable]
        public int? DoctorId { get; set; }
        [NotNull]
        public string Nome { get; set; }
        [NotNull]
        public string Cognome { get; set; }
        [NotNull]
        public string Mail { get; set; }
        [Nullable]
        public DateTime? GirnoAssociazione { get; set; }
    }
}
  
