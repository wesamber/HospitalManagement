using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Patients;
using HospitalManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services;

public class PatientService : IPatientService
{
    public Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<PatientListDto>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<PatientDto>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(UpdatePatientDto dto)
    {
        throw new NotImplementedException();
    }
}
