//using HospitalManagement.Application.Interfaces.Persistence;
//using HospitalManagement.Application.Interfaces.Repositories;
//using HospitalManagement.Domain.Entities.Departments;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HospitalManagement.Infrastructure.Persistence.Ado;

//public class AdoDepartmentRepository : AdoRepository<Department> , IDepartmentRepository
//{
//    public AdoDepartmentRepository(ISqlConnectionFactory connectionFactory) 
//        : base(connectionFactory)
//    {
//    }

//    public async Task<Department?> GetByIdAsync(Guid id)
//    {
//        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();
//        return await LoadDepartmentAsync(conn,
//            "WHERE Id = @Id",
//            cmd => AddGuidParam(cmd, "@Id", id));
//    }


//}
