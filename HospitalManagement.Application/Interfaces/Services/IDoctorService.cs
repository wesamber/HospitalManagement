using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Services;

public interface IDoctorService
{
    Task<Result<DoctorDto>> CreateAsync(CreateDoctorDto dto);
    Task<Result<bool>> UpdateAsync(UpdateDoctorDto dto); 
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<DoctorDetailsDto>> GetByIdAsync(Guid id);
    Task<Result<List<DoctorDto>>> GetAllAsync();
}
