using AutoMapper;
using HospitalManagement.Application.DTOs.Treatments;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Mappers.Treatments;

public class TreatmentProfile : Profile
{
    public TreatmentProfile()
    {
        CreateMap<TreatmentInternal , TreatmentInternalDto>()
            .ForMember(d => d.Type , opt => opt.MapFrom(s => "internal"));

        CreateMap<TreatmentExternal, TreatmentExternalDto>()
           .ForMember(d => d.Type, o => o.MapFrom(_ => "external"));

        CreateMap<Treatment, TreatmentListDto>()
            .ForMember(d => d.Type, o => o.MapFrom(
                s => s is TreatmentInternal ? "internal" : "external"));

        CreateMap<DoctorTreatment, DoctorTreatmentDto>()
            .ForMember(d => d.TreatmentRole, o => o.MapFrom(
                s => s.RoleInTreatment.ToString()));
    }
}
