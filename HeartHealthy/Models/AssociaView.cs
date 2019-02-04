using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class AssociaView
    {
        public int DoctorId { get; set; }
        public int FitBitId { get; set; }
        public string NomeDoctor { get; set; }
        public string CognomeDoctor { get; set; }
    }
}
