using AutoMapper;
using HospitalManagement.Application.DTOs.Departments;
using HospitalManagement.Domain.Entities.Departments;

namespace HospitalManagement.Application.Mappers.Departments;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentDto>();
    }
}
