using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Doctors;

public class ContractedRole : DoctorRole , IEntity
{
    public override string RoleName => "Contracted";
    public decimal Percent { get; private set; } = 0.5m; // 50%

    private readonly List<Guid> _treatmentIds = new();
    public IReadOnlyList<Guid> TreatmentIds => _treatmentIds.AsReadOnly();

    private ContractedRole() { }

    // constructor for loading 
    public ContractedRole(Guid id , DateTime startDate , DateTime? endDate ,bool isActive ,  decimal percent)
        : base(id, startDate , endDate , isActive)
    {
        Percent = percent;      
    }

    // constructor for creating new contracted role
    public ContractedRole(DateTime startDate , DateTime? endDate , decimal percent)
        : base( Guid.NewGuid(), startDate , null , true)
    {
        Percent = percent;
    }

    public override decimal CalculateSalary(decimal totalTreatmentsCost)
    {
        // راتب الطبيب المتعاقد يعتمد بشكل كامل على العلاجات التي يقدمها، وليس له راتب ثابت
        return totalTreatmentsCost * Percent; 
    }
}
