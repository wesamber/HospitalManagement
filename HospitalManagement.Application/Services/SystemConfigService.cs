using HospitalManagement.Application.Common;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class SystemConfigService : ISystemConfigService
{
    private readonly ISystemConfigRepository _repo;

    public SystemConfigService(ISystemConfigRepository repo)
        => _repo = repo;

    public async Task<Result<decimal>> GetBaseSalaryAsync()
    {
        var config = await _repo.GetAsync();
        return Result<decimal>.SuccessResult(config.BaseSalary);
    }

    public async Task<Result<bool>> UpdateBaseSalaryAsync(decimal newSalary)
    {
        if (newSalary <= 0)
            return Result<bool>.Failure("Salary must be positive.");

        var config = await _repo.GetAsync();
        config.UpdateBaseSalary(newSalary);
        await _repo.UpdateAsync(config);
        return Result<bool>.SuccessResult(true);
    }
}
