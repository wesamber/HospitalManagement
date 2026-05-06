using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Departments;
using HospitalManagement.Domain.Entities.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<Result<List<DepartmentDto>>> GetAllAsync();
    Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
    Task<Result<DepartmentDto>> GetByIdAsync(Guid id);
    Task<Result<bool>> UpdateAsync(UpdateDepartmentDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<bool>> AdmitPatientAsync(Guid departmentId, Guid patientId);
    Task<Result<bool>> RemovePatientAsync(Guid departmentId, Guid patientId);

}
