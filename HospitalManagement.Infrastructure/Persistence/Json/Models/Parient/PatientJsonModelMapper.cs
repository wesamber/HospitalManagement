using HospitalManagement.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Parient;

public static class PatientJsonModelMapper
{
    public static PatientJsonModel ToJsonModel(this Patient patient)
    {
        var model = new PatientJsonModel
        {
            Id = patient.Id,
            PatientNumber = patient.PatientNumber,
            Name = patient.Name,
            DateOfBirth = patient.DateOfBirth,
            Address = patient.Address,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            ExternalTreatmentIds = patient.ExternalTreatmentIds.ToList(),
            Type = patient.Type ?? (patient is InternalPatient ? "internal" : "external")
        };

        if (patient is InternalPatient internal_)
        {
            model.IsDischarged = internal_.IsDischarged;
            model.DischargeDate = internal_.DischargeDate;
            model.DepartmentId = internal_.DepartmentId;
            model.InternalTreatmentIds = internal_.InternalTreatments.ToList();
        }

        if (patient is ExternalPatient external_)
        {
            model.IsAdmitted = external_.IsAdmitted;
            model.AdmissionDate = external_.AdmissionDate;
        }

        return model;
    }

    public static Patient ToDomain(this PatientJsonModel model)
    {
        if (model.Type == "internal")
        {
            var patient = new InternalPatient(
                id: model.Id,
                patientNumber: model.PatientNumber,
                name: model.Name,
                dateOfBirth: model.DateOfBirth,
                address: model.Address,
                phone: model.PhoneNumber,
                email: model.Email,
                departmentId: model.DepartmentId!.Value,
                internalTreatments: model.InternalTreatmentIds,
                externalTreatments: model.ExternalTreatmentIds,
                isDischarged: model.IsDischarged,
                dischargeDate: model.DischargeDate);

            return patient;
        }
        else
        {
            var patient = new ExternalPatient(
                patientId: model.Id,
                patientNumber: model.PatientNumber,
                name: model.Name,
                dateOfBirth: model.DateOfBirth,
                address: model.Address,
                phone: model.PhoneNumber,
                email: model.Email,
                externalTreatmentIds: model.ExternalTreatmentIds,
                isAdmitted: model.IsAdmitted,
                admissionDate: model.AdmissionDate);

            return patient;
        }
    }
}
