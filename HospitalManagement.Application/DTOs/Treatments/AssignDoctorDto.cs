using HospitalManagement.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Treatments;

public class AssignDoctorDto
{
    public Guid TreatmentId { get; set; }
    public Guid DoctorId { get; set; }
    public string RoleInTreatment { get; set; } = string.Empty;
}
