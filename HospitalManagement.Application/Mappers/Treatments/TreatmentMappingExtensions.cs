using HospitalManagement.Application.DTOs.Treatments;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalManagement.Application.Mappers.Treatments;

public static class TreatmentMappingExtensions
{
    public static TreatmentInternal ToDomain(this CreateTreatmentInternalDto dto, string number)
    {
        return new TreatmentInternal(
            numberTreatment: number,
            datestart: dto.StartDate,
            cost: dto.Cost,
            dateDicharge: dto.DateDischarge,
            departmentId: dto.DepartmentId,
            patientId: dto.PatientId);
    }

    public static TreatmentExternal ToDoamin(this  CreateTreatmentExternalDto dto, string number)
    {
        return new TreatmentExternal(
            numberTreatment: number,
            startDate: dto.StartDate,
            cost: dto.Cost,
            clinicNumber: dto.ClinicNumber,
            doctorName: dto.DoctorName,
            patientId: dto.PatientId);
    }
}
