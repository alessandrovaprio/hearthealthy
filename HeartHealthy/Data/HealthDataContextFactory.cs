using LinqToDB.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartHealthy.Data
{
    public class HealthDataContextFactory : IDataContextFactory<HealthDataContext>
    {
        readonly IDataProvider dataProvider;

        readonly string connectionString;

        public HealthDataContextFactory(IDataProvider dataProvider, string connectionString)
        {
            this.dataProvider = dataProvider;
            this.connectionString = connectionString;
        }

        public HealthDataContext Create() => new HealthDataContext(dataProvider, connectionString);
    }
}
