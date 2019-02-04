using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class FitbitView
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Mail { get; set; }
        public string NomeDottore { get; set; }
        public string CognomeDottore { get; set; }
    }
}
