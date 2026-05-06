using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagement.Domain.Entities.Departments;
using HospitalManagement.Infrastructure.Persistence.Json.Models.Departments;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Departments;

public static class DepartmentJsonModelMapper
{
    public static DepartmentJsonModel ToJsonModel(this Department department)
    {
        return new DepartmentJsonModel
        {
            Id = department.Id,
            Name = department.Name,
            DoctorIds = department.DoctorIds.ToList(),
            PatientIds = department.PatientIds.ToList()
        };
    }

    public static Department ToDomain(this DepartmentJsonModel model)
    {
        return new Department(
            id: model.Id,
            name: model.Name,
            patientIds: model.PatientIds,
            doctorIds: model.DoctorIds
        );
    }

}