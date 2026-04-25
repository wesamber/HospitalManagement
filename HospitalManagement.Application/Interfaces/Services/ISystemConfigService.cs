using HospitalManagement.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Services;

public interface ISystemConfigService
{
    Task<Result<decimal>> GetBaseSalaryAsync();
    Task<Result<bool>> UpdateBaseSalaryAsync(decimal newSalary);
}
