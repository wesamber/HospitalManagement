
using HospitalManagement.Domain.Entities.Departments;
using HospitalManagement.Domain.Entities.Enums;
using System.Text.Json.Serialization;

namespace HospitalManagement.Domain.Entities.Doctors;

public class Doctor
{
    public Guid DoctorId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string DoctorNumber { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Specialization Specialization { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }

    private readonly List<Guid> _departmentsIds = new();
    public IReadOnlyList<Guid> DepartmentsIds => _departmentsIds.AsReadOnly();

    private readonly List<DoctorTreatment> _treatments = new();
    public IReadOnlyList<DoctorTreatment> Treatments => _treatments.AsReadOnly();

    private readonly List<DoctorRole> _roles = new();
    public IReadOnlyList<DoctorRole> Roles => _roles.AsReadOnly();

    public DoctorRole? ActiveRole => _roles?.FirstOrDefault(r => r.IsActive);
    
    protected Doctor() { }

    public Doctor(Guid id, string name, string doctorNumber, Specialization specialization, DateOnly dob)
    {
        DoctorId = id;
        Name = name;
        DoctorNumber = doctorNumber;
        Specialization = specialization;
        DateOfBirth = dob;
    }
    public void AddRole(DoctorRole role)
    {
        ActiveRole?.Deactivate(DateTime.Now);
        _roles.Add(role);
    }

    public void AssignToDepartment(Guid departmentId)
    {
        if (_departmentsIds.Contains(departmentId))
            throw new InvalidOperationException($"Doctor is already assigned to department with ID {departmentId}.");

        _departmentsIds.Add(departmentId);
    }

    public void PromoteToPermanent(decimal baseSalary)
    {
        ActiveRole?.Deactivate(DateTime.Now);
        _roles.Add(new PermanentRole(Guid.NewGuid(), DateTime.Now, baseSalary));
    }

    public void AddTreatment(DoctorTreatment treatment)
    {
        if (!_treatments.Any(t => t.TreatmentId == treatment.TreatmentId))
            _treatments.Add(treatment);
    }

    public decimal CalculateSalary(decimal systemBaseSalary)
    {
        // الراتب الاساسي او شي بيخصو بيجي من طبقة ال application 
        if (ActiveRole is null)
            throw new InvalidOperationException("No active role to calculate salary.");

        return ActiveRole.CalculateSalary(systemBaseSalary);
    }

}
