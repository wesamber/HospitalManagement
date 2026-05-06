using AutoMapper;
using HospitalManagement.Application.DTOs.Patients;
using HospitalManagement.Domain.Entities.Patients;
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
        CreateMap<InternalPatient, InternalPatientDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "internal"));

        CreateMap<ExternalPatient, ExternalPatientDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "external"));

        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                src is InternalPatient ? "internal" : "external"));

        CreateMap<Patient, PatientListDto>()
            .Include<InternalPatient, PatientListDto>()
            .Include<ExternalPatient, PatientListDto>();

        CreateMap<InternalPatient, PatientListDto>()
    .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "internal"));

        CreateMap<ExternalPatient, PatientListDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "external"));
    }
}
