using AutoMapper;
using HospitalManagement.Application.DTOs.Doctors;
using HospitalManagement.Application.DTOs.Doctors.DoctorRoles;
using HospitalManagement.Application.DTOs.Treatments;
using HospitalManagement.Domain.Entities.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Mappers.Doctors;

public class DoctorProfile : Profile
{
    public DoctorProfile()
    {
        CreateMap<SalaryRecord, SalaryRecordDto>();

        CreateMap<Doctor, DoctorDto>()
             .ForMember(d => d.Specialization,
                 opt => opt.MapFrom(src => src.Specialization.ToString()))
             .ForMember(d => d.ActiveRole,
                 opt => opt.MapFrom(src => src.ActiveRole != null ? src.ActiveRole.RoleName : null));

        CreateMap<Doctor, DoctorDetailsDto>()
            .ForMember(d => d.Specialization,
                opt => opt.MapFrom(src => src.Specialization.ToString()))
            .ForMember(d => d.ActiveRole,
                opt => opt.MapFrom(src => src.ActiveRole != null ? src.ActiveRole.RoleName : null))
            .ForMember(d => d.Roles,
                opt => opt.MapFrom(src => src.Roles))
            .ForMember(d => d.Treatments,
                opt => opt.MapFrom(src => src.Treatments))
            .ForMember(d => d.Departments,
                opt => opt.Ignore());

        CreateMap<DoctorTreatment, DTOs.Doctors.DoctorTreatmentDto>()
            .ForMember(d => d.TreatmentRole,
             opt => opt.MapFrom(src => src.RoleInTreatment.ToString()));

        CreateMap<DoctorRole, DoctorRoleDto>()
            .Include<PermanentRole, PermanentRoleDto>()
            .Include<TraineeRole, TraineeRoleDto>()
            .Include<ContractedRole, ContractRoleDto>()
            .ForMember(d => d.SalaryHistory, opt => opt.MapFrom(src => src.SalaryHistory.ToList()));

        CreateMap<PermanentRole, PermanentRoleDto>()
             .IncludeBase<DoctorRole, DoctorRoleDto>();

        CreateMap<TraineeRole, TraineeRoleDto>()
            .IncludeBase<DoctorRole, DoctorRoleDto>();

        CreateMap<ContractedRole, ContractRoleDto>()
            .IncludeBase<DoctorRole, DoctorRoleDto>();

        CreateMap<DoctorTreatment, DTOs.Treatments.DoctorTreatmentDto>()
           .ForMember(d => d.TreatmentRole,
               opt => opt.MapFrom(src => src.RoleInTreatment.ToString()));
    }
}
