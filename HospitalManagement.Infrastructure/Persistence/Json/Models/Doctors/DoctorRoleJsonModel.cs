using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Doctors;

public class DoctorRoleJsonModel
{
    public Guid Id { get; set; }
    public string RoleType { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }

    public decimal? BaseSalary { get; set; } // PermanentRole
    public decimal? Percent { get; set; } // ContractedRole
    public List<Guid> TreatmentIds { get; set; } = new();
    public List<SalaryRecordJsonModel> SalaryHistory { get; set; } = new();
}
