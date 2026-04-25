using HospitalManagement.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities.Config;

public class SystemConfig : IEntity
{
    public Guid Id {  get; private set; }
    public decimal BaseSalary { get; private set; }
    protected SystemConfig() { }

    public SystemConfig(decimal baseSalary)
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        BaseSalary = baseSalary;
    }

    public void UpdateBaseSalary(decimal newSalary)
    {
        if (newSalary <= 0)
            throw new ArgumentException("Base salary must be positive.");
        BaseSalary = newSalary;
    }
}
