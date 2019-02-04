using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Models
{
    public class MonitoraggioModels
    {
        public Fitbit Paziente { get; set; }
        public List<Battito> BattitiperPaziente { get; set; }
        public List<Percorso> Percorsi { get; set; }
        public List<Peso> Peso { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public string DateCoordBattiti { get; set; }
        public string CoordBattiti { get; set; }
        public string DateCoordPercorso { get; set; }
        public string CoordPercorso { get; set; }
        public string CoordPassi { get; set; }
        public string CoorddurataAttività { get; set; }
        public string DateCoordPeso { get; set; }
        public string CoordPeso { get; set; }
    }
    public class DataMonitoraggio
    {
        public int Paziente { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
    }
}
