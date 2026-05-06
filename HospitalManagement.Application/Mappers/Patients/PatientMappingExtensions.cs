using HospitalManagement.Application.DTOs.Patients;
using HospitalManagement.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Mappers.Patients;

public static class PatientMappingExtensions
{
    public static InternalPatient ToDomain(this CreatePatientInternalDto dto , string patientnumber)
    {
        return new InternalPatient(
            patientNumber: patientnumber,
            name: dto.Name,
            dateOfBirth: dto.DateOfBirth,
            address: dto.Address,
            phone: dto.PhoneNumber,
            email: dto.Email,
            departmentId: dto.DepartmentId);
    }

    public static ExternalPatient ToDomain(this CreatePatientExternalDto dto , string patientnumber)
    {
        return new ExternalPatient(
            patientNumber: patientnumber,
            name: dto.Name,
            dateOfBirth: dto.DateOfBirth,
            address: dto.Address,
            phone: dto.PhoneNumber,
            email: dto.Email);
    }

    public static InternalPatient ToInternalFromExternal(this ExternalPatient externalPatient , Guid departmentId)
    {
        return new InternalPatient(
            id : externalPatient.Id,
            patientNumber: externalPatient.PatientNumber,
            name: externalPatient.Name,
            dateOfBirth: externalPatient.DateOfBirth,
            address: externalPatient.Address,
            phone: externalPatient.PhoneNumber,
            email: externalPatient.Email,
            departmentId: departmentId,
            externalTreatments: externalPatient.ExternalTreatmentIds,
            internalTreatments: null,
            isDischarged: false,
            dischargeDate: null);
    }
}
