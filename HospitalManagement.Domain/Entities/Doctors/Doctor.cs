
using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Enums;

namespace HospitalManagement.Domain.Entities.Doctors;

public class Doctor : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string DoctorNumber { get; private set; } = string.Empty;
    public Specialization Specialization { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string? Address { get; private set; }
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

    // constructor for loading 
    public Doctor(
        Guid id, string name, string doctorNumber,
        Specialization specialization, DateOnly dob , 
        string? address , string? phoneNumber , string? email)
    {
        Id = id;
        Name = name;
        DoctorNumber = doctorNumber;
        Specialization = specialization;
        DateOfBirth = dob;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    // constructor for creating new doctor
    public Doctor(
        string name, string doctorNumber,
        Specialization specialization, DateOnly dob , 
        string? address , string? phoneNumber , string? email)
        :this(Guid.NewGuid() , name, doctorNumber, specialization, dob, address, phoneNumber, email)
    {
        Name = name;
        DoctorNumber = doctorNumber;
        Specialization = specialization;
        DateOfBirth = dob;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public void UpdateInfo(string name, Specialization specialization, DateOnly dob, string? address, string? phoneNumber, string? email)
    {
        Name = name;
        Specialization = specialization;
        DateOfBirth = dob;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
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
        // التأكد أن الدور "النشط حالياً" هو دور متدرب
        if (ActiveRole is not TraineeRole)
            throw new InvalidOperationException("Only doctors currently in a Trainee role can be promoted to Permanent.");

        // إيقاف الدور الحالي (المتدرب)
        ActiveRole.Deactivate(DateTime.Now);

        // إضافة الدور الدائم الجديد
        var permanentRole = new PermanentRole(DateTime.Now, null, baseSalary);

        permanentRole.ArchiveCurrentSalary(baseSalary);

        _roles.Add(permanentRole);
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

    internal void LoadDepartmentIds(List<Guid> departs)
    {
        _departmentsIds.AddRange(departs);
    }

    internal void LoadTreatments(List<DoctorTreatment> treatments)
    {
        _treatments.AddRange(treatments);
    }

    internal void LoadRoles(List<DoctorRole> roles)
    {
        _roles.AddRange(roles);
    }
}
