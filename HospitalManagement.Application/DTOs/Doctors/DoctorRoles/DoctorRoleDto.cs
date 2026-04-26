using HospitalManagement.Domain.Entities.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors.DoctorRoles;

public abstract class DoctorRoleDto
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }

    public List<SalaryRecordDto> SalaryHistory { get; set; } = new();
}
