using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors.DoctorRoles;

public class PermanentRoleDto : DoctorRoleDto
{
    public decimal BaseSalary { get; set; }
}
