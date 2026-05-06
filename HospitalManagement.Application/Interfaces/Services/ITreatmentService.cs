using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Services;

public interface ITreatmentService
{
    Task<Result<TreatmentInternalDto>> CreateInternalAsync(CreateTreatmentInternalDto dto);
    Task<Result<TreatmentExternalDto>> CreateExternalAsync(CreateTreatmentExternalDto dto);

    Task<Result<object>> GetByIdAsync(Guid id);

    Task<Result<bool>> AssignDoctorAsync(AssignDoctorDto dto);
    Task<Result<bool>> DischargeAsync(Guid treatmentId, DateTime dischargeDate);
    Task<Result<bool>> DeleteAsync(Guid id);

    Task<Result<List<TreatmentListDto>>> GetAllAsync();
    Task<Result<List<TreatmentListDto>>> GetByPatientIdAsync(Guid patientId);
    Task<Result<List<TreatmentListDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Result<List<TreatmentListDto>>> GetByDoctorAsync(Guid doctorId);
    Task<Result<List<TreatmentListDto>>> GetByDoctorAndPeriodAsync(Guid doctorId, DateTime startDate, DateTime endDate);
}
