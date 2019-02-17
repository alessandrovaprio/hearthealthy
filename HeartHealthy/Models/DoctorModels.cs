using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class DoctorModels
    {
        public List<Fitbit> fitbits { get; set; }
        public List<Allenamento> Allenamenti { get; set; }
        public int AllenmentiSettimana { get; set; }
        public int Battiti { get; set; }
        public int AllenamentiTot { get; set; }

        
    }
}
