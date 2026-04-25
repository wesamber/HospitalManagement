using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Services;

public interface IDoctorService
{
    #region CRUD Operations Doctors
    Task<Result<DoctorDto>> CreateAsync(CreateDoctorDto dto);
    Task<Result<bool>> UpdateAsync(UpdateDoctorDto dto); 
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<DoctorDetailsDto>> GetByIdAsync(Guid id);
    Task<Result<List<DoctorDto>>> GetAllAsync();
    #endregion

    #region GET By Properties
    Task<Result<List<DoctorDto>>> GetBySpecializationAsync(string specialization);
    Task<Result<List<DoctorDto>>> GetByDepartmentAsync(Guid departmentId);
    Task<Result<DoctorDto>> GetByNumberAsync(string doctorNumber);
    #endregion

    #region METHODS DOMAIN
    Task<Result<bool>> AssignToDepartmentAsync(Guid doctorId, Guid departmentId);
    Task<Result<bool>> AddTreatmentToDoctorAsync(Guid doctorId, AddDoctorTreatmentDto dto);
    Task<Result<decimal>> CalculateSalaryAsync(Guid doctorId);
    Task<Result<bool>> PromoteDoctorToPermanentAsync(Guid doctorId, decimal? baseSalary = null);
    Task<Result<bool>> AddRoleToDoctorAsync(Guid doctorId, AddRoleDoctorDto dto);

    #endregion
}
