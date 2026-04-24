using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class TreatmentInternalDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public decimal Cost { get; set; }
    public DateTime? DateDischarge { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PatientId { get; set; }
    public List<DoctorTreatmentDto> Doctors { get; set; } = new();
}