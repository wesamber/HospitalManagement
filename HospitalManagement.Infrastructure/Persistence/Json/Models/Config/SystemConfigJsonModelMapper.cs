using HospitalManagement.Domain.Entities.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Config;

public static class SystemConfigJsonModelMapper
{
    public static SystemConfigJsonModel ToJsonModel(this SystemConfig config)
        => new() { Id = config.Id, BaseSalary = config.BaseSalary };

    public static SystemConfig ToDomain(this SystemConfigJsonModel model)
    {
        var config = new SystemConfig(model.BaseSalary);
        return config;
    }
}
