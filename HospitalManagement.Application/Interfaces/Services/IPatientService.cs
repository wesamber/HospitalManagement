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
    // CRUD
    Task<Result<InternalPatientDto>> CreateInternalAsync(CreatePatientInternalDto dto);
    Task<Result<ExternalPatientDto>> CreateExternalAsync(CreatePatientExternalDto dto);
    Task<Result<List<PatientListDto>>> GetAllAsync();
    Task<Result<object>> GetByIdAsync(Guid id); // بيرجع Internal أو External حسب النوع
    Task<Result<bool>> UpdateAsync(UpdatePatientDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);

    // Domain Actions
    Task<Result<InternalPatientDto>> AdmitExternalPatientAsync(Guid patientId, Guid departmentId);
    Task<Result<bool>> DischargeAsync(Guid patientId);
}
