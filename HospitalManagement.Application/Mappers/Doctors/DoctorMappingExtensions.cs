using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Mappers.Doctors;

public static class DoctorMappingExtensions
{
    public static Doctor ToEntity(this CreateDoctorDto dto , string doctorNumber)
    {
        return new Doctor(
            name: dto.Name,
            doctorNumber: doctorNumber,
            specialization: Enum.TryParse<Specialization>(dto.Specialization, out var spec)
                ? spec
                : throw new ArgumentException("Invalid specialization"),
            dob: dto.DateOfBirth,
            address: dto.Address,
            phoneNumber: dto.PhoneNumber,
            email: dto.Email
        );
    }

    public static AddRoleDoctorDto SplitRoleFromDto(this CreateDoctorDto dto)
    {
        return new AddRoleDoctorDto
        {
            RoleName = dto.RoleName,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            BaseSalary = dto.BaseSalary,
            Percent = dto.Percent
        };
    }
}
