using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Doctors;

public class ContractedRole : DoctorRole 
{
    public override string RoleName => "contracted";
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
        : base( startDate , endDate)
    {
        Percent = percent;
        ArchiveCurrentSalary(percent);
    }

    public override decimal CalculateSalary(decimal totalTreatmentsCost)
    {
        // راتب الطبيب المتعاقد يعتمد بشكل كامل على العلاجات التي يقدمها، وليس له راتب ثابت
        return totalTreatmentsCost * Percent; 
    }

    internal void LoadTreatmentIds(IEnumerable<Guid> treatmentIds)
    {
        _treatmentIds.AddRange(treatmentIds);
    }

}
