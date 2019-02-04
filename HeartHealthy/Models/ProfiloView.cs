using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class ProfiloView
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Ruolo { get; set; }
    }
}
