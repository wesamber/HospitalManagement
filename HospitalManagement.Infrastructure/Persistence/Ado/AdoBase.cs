//using Microsoft.Data.SqlClient;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HospitalManagement.Infrastructure.Persistence.Ado;

//public abstract class AdoBase
//{
//    private readonly string _connectionString;

//    protected AdoBase(string connectionString)
//    {
//        _connectionString = connectionString;
//    }

//    // كل Repository بستخدمه لفتح connection
//    protected SqlConnection CreateConnection()
//        => new SqlConnection(_connectionString);
//}