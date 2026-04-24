using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Services;

public interface IPatientService
{
    Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto);
    Task<Result<bool>> UpdateAsync(UpdatePatientDto dto); 
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<PatientDto>> GetByIdAsync(Guid id);
    Task<Result<List<PatientListDto>>> GetAllAsync();
}
