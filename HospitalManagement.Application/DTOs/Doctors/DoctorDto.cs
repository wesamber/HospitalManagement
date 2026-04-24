using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors;

public class DoctorDto 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DoctorNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
}
