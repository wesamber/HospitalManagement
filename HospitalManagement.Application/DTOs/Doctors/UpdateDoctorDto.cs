using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors;

public class UpdateDoctorDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }   
    public string? PhoneNumber { get; set; }   
    public string? Email { get; set; }
}
