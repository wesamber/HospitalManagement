using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Departments;

public class DepartmentPatientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PatientNumber { get; set; } = string.Empty;
}
