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
        CreateMap<TreatmentInternal, TreatmentInternalDto>()
    .ForMember(d => d.Type, opt => opt.MapFrom(_ => "internal"))
    .ForMember(d => d.PatientId, opt => opt.MapFrom(s => s.PatientId));

        CreateMap<TreatmentExternal, TreatmentExternalDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(_ => "external"))
            .ForMember(d => d.PatientId, opt => opt.MapFrom(s => s.PatientId));

        CreateMap<Treatment, TreatmentListDto>()
            .ForMember(d => d.Type, o => o.Ignore())
            .ForMember(d => d.PatientId, o => o.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.Type = src is TreatmentInternal ? "internal" : "external";

                if (src is TreatmentInternal ti)
                    dest.PatientId = ti.PatientId;
                else if (src is TreatmentExternal te)
                    dest.PatientId = te.PatientId;
                else
                    dest.PatientId = Guid.Empty;
            });


        CreateMap<DoctorTreatment, DoctorTreatmentDto>()
            .ForMember(d => d.TreatmentRole, o => o.MapFrom(
                s => s.RoleInTreatment.ToString()));
    }
}
