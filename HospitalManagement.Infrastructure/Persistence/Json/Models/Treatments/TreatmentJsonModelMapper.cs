using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Treatments;

public static class TreatmentJsonModelMapper
{
    public static TreatmentJsonModel ToJsonModel(this Treatment treatment)
    {
        var model = new TreatmentJsonModel
        {
            Id = treatment.Id,
            NumberTreatment = treatment.NumberTreatment,
            StartDate = treatment.StartDate,
            Cost = treatment.Cost,
            Type = treatment is TreatmentInternal ? "internal" : "external",
        };

        if (treatment is TreatmentInternal i)
        {
            model.PatientId = i.PatientId;
            model.DateDischarge = i.DateDischarge;
            model.DepartmentId = i.DepartmentId;
            model.Doctors = i.Doctors.Select(d => new DoctorTreatmentJsonModel
            {
                DoctorId = d.DoctorId,
                TreatmentId = d.TreatmentId,
                Role = d.RoleInTreatment.ToString()
            }).ToList();
        }

        if (treatment is TreatmentExternal e)
        {
            model.PatientId = e.PatientId;
            model.ClinicNumber = e.ClinicNumber;
            model.DoctorName = e.DoctorName;
        }

        return model;
    }

    public static Treatment ToDomain(this TreatmentJsonModel model)
    {
        if (model.Type == "internal")
        {
            var treatment = new TreatmentInternal(
                id: model.Id,
                numberTreatment: model.NumberTreatment,
                datestart: model.StartDate,
                cost: model.Cost,
                dateDicharge: model.DateDischarge ?? DateTime.MinValue,
                departmentId: model.DepartmentId!.Value,
                patientId: model.PatientId);

            foreach (var d in model.Doctors)
            {
                var role = Enum.Parse<TreatmentRole>(d.Role);
                treatment.AddDoctor(new DoctorTreatment(d.DoctorId, d.TreatmentId, role));
            }

            return treatment;
        }
        else
        {
            return new TreatmentExternal(
                id: model.Id,
                numberTreatment: model.NumberTreatment,
                startDate: model.StartDate,
                cost: model.Cost,
                clinicNumber: model.ClinicNumber!,
                doctorName: model.DoctorName!,
                patientId: model.PatientId);
        }
    }
}