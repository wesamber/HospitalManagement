using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Domain.Entities.Doctors;

public abstract class DoctorRole : IEntity
{
    public Guid Id { get; private set; }
    public abstract string RoleName { get; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }

    // لتخزين جميع الرواتب مع الوقت 
    private readonly List<SalaryRecord> _salaryHistory = new();
    public IReadOnlyList<SalaryRecord> SalaryHistory => _salaryHistory.AsReadOnly();

    protected DoctorRole() { }

    // For creating new role 
    protected DoctorRole(DateTime startDate , DateTime? endDate) 
        : this(Guid.NewGuid(), startDate , endDate , true)
    {
        IsActive = true;
    }
    // وقت تحميل البيانات
    protected DoctorRole(Guid id, DateTime startDate, DateTime? endDate, bool isActive)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }
    public void Deactivate(DateTime endDate)
    {
        if(IsActive == false)
            throw new InvalidOperationException($"Role '{RoleName}' is already inactive.");

        if(endDate < StartDate)
            throw new ArgumentException($"End date cannot be before start date.");

        IsActive = false;
        EndDate = endDate;
    }

    public void ArchiveCurrentSalary(decimal amount)
    {
        _salaryHistory.Add(new SalaryRecord(DateTime.Now, amount));
    }
    public abstract decimal CalculateSalary(decimal referenceAmount);

    // هي للنخزين بدون معالجة الملفات بس اضافة
    internal void LoadSalaryRecord(SalaryRecord record)
    {
        _salaryHistory.Add(record);
    }

}

