using HospitalManagement.Application.DTOs.Departments;
using HospitalManagement.Domain.Entities.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Mappers.Departments;

public static class DepartmentMappingExtensions
{
    public static Department ToEntity(this CreateDepartmentDto dto)
    {
        return new Department(dto.Name);
    }
}
