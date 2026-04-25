using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Config;

public class SystemConfigJsonModel
{
    public Guid Id { get; set; }
    public decimal BaseSalary { get; set; }
}
