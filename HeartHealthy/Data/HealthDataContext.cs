using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using HeartHealthy.Models;

namespace HeartHealthy.Data
{
    public class HealthDataContext : DataConnection
    {
        public HealthDataContext(IDataProvider dataProvider, string connectionString)
            : base(dataProvider, connectionString)
        { }

        public ITable<Users> GetUsers => GetTable<Users>();
        public ITable<Fitbit> GetFitbits => GetTable<Fitbit>();
        public ITable<Battito> GetBattito => GetTable<Battito>();
        public ITable<Allenamento> GetAllenamento => GetTable<Allenamento>();
        public ITable<Peso> GetPeso => GetTable<Peso>();
        public ITable<Attivita> GetAttivita => GetTable<Attivita>();
        public ITable<Percorso> GetPercorso => GetTable<Percorso>();
    }
    
}
