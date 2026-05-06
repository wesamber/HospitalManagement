using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Departments;

public class DepartmentJsonModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Guid> DoctorIds { get; set; } = new();
    public List<Guid> PatientIds { get; set; } = new();
}
