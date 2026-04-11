using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities.Doctors;

public class ContractedRole : DoctorRole
{
    public override string RoleName => "Contracted";
    public decimal Percent { get; private set; } = 0.5m; // 50%

    private readonly List<Guid> _treatmentIds = new();
    public IReadOnlyList<Guid> TreatmentIds => _treatmentIds.AsReadOnly();

    protected ContractedRole() { }

    public ContractedRole(Guid id , DateTime startDate , decimal percent)
        : base(id, startDate)
    {
        Percent = percent;      
    }

    public override decimal CalculateSalary(decimal totalTreatmentsCost)
    {
        // راتب الطبيب المتعاقد يعتمد بشكل كامل على العلاجات التي يقدمها، وليس له راتب ثابت
        return totalTreatmentsCost * Percent; 
    }
}
