using HospitalManagement.Domain.Entities.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface ISystemConfigRepository
{
    Task<SystemConfig> GetAsync();
    Task UpdateAsync(SystemConfig config);
}
