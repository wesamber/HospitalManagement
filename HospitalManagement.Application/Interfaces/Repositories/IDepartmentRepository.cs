using HospitalManagement.Domain.Entities.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
   Task<Department?> GetByNameAsync(string name);
   Task<bool> ExistsAsync(string name);
}
