using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Eventify_High_Performance_Event_Management_API.Data
{

    public class DataContext
    {
        private readonly IConfiguration _config;
        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return db.Query<T>(sql);
        }
        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return db.QuerySingle<T>(sql);
        }
        public bool ExecuteSql(string sql)
        {
            IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return db.Execute(sql) > 0;
        }
        public int ExecuteSqlWithCountRow(string sql)
        {
            IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return db.Execute(sql);
        }
    }
}