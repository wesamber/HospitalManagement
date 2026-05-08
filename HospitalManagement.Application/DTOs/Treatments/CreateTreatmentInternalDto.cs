using HospitalManagement.Application.DTOs.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class CreateTreatmentInternalDto
{
    public string Name { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid DepartmentId { get; set; }
    public decimal Cost { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DateDischarge { get; set; }
}
