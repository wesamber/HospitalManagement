using AutoMapper;
using HospitalManagement.Application.DTOs.Doctors;
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
        CreateMap<Doctor, DoctorDto>();
        CreateMap<Doctor , DoctorDetailsDto>();
    }
}
