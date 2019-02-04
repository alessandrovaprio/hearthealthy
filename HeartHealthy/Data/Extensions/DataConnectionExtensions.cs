using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.Sqlite;
using System;


namespace HeartHealthy.Data.Extensions
{
    public static class DataConnectionExtensions
    {
        public static void CreateTableIfNotExists<T>(this DataConnection db) => RunIgnoringException(() => db.CreateTable<T>());

        private static void RunIgnoringException(Action action)
        {
            if (action == null) return;

            try
            {
                action.Invoke();
            }
            catch (SqliteException)
            { }
        }
    }
}
