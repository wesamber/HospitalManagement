using AutoMapper;
using HospitalManagement.Application.DTOs.Patients;
using HospitalManagement.Domain.Entities.Patients;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Mappers.Patients;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<InternalPatient, PatientListDto>()
           .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "internal"));

        CreateMap<ExternalPatient, PatientListDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "external"));

        // Base map مع Include — لازم يكون موجود لأنو GetAllAsync بترجع List<Patient>
        CreateMap<Patient, PatientListDto>()
            .Include<InternalPatient, PatientListDto>()
            .Include<ExternalPatient, PatientListDto>();

        // Details
        CreateMap<InternalPatient, InternalPatientDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "internal"))
            .ForMember(dest => dest.InternalTreatments, opt => opt.Ignore())
            .ForMember(dest => dest.ExternalTreatments, opt => opt.Ignore());

        CreateMap<ExternalPatient, ExternalPatientDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "external"))
            .ForMember(dest => dest.ExternalTreatments, opt => opt.Ignore());

        // Treatments
        CreateMap<TreatmentInternal, PatientTreatmentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "internal"))
            .ForMember(dest => dest.ClinicNumber, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorName, opt => opt.Ignore());

        CreateMap<TreatmentExternal, PatientTreatmentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "external"))
            .ForMember(dest => dest.DateDischarge, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.Ignore());
    }
}
