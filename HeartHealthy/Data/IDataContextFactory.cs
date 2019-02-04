using LinqToDB.Data;


namespace HeartHealthy.Data
{
    public interface IDataContextFactory<T>
       where T : DataConnection
    {
        T Create();
    }
}
