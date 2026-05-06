using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Departments;

public class DepartmentDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DepartmentDoctorDto> Doctors { get; set; } = new();
    public List<DepartmentPatientDto> Patients { get; set; } = new();
}
