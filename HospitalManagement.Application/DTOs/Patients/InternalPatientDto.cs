using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.DTOs.Patients;

public class InternalPatientDto : PatientDto
{
    public Guid DepartmentId { get; set; }
    public bool IsDischarged { get; set; }
    public DateTime? DischargeDate { get; set; }
    public List<Guid> InternalTreatmentIds { get; set; } = new();
}
