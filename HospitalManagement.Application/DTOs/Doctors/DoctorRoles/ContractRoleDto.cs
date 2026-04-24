using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Doctors.DoctorRoles;

public class ContractRoleDto : DoctorRoleDto
{
    public decimal Percent { get; set; }
    public List<Guid> TreatmentIds { get; set; } = new ();
}
