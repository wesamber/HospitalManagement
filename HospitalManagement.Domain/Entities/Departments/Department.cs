using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Patients;

namespace HospitalManagement.Domain.Entities.Departments;

public class Department : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private readonly List<Guid> _patientIds = new();
    public IReadOnlyList<Guid> PatientIds => _patientIds.AsReadOnly();

    private readonly List<Guid> _doctorIds = new();
    public IReadOnlyList<Guid> DoctorIds => _doctorIds.AsReadOnly();

    public Department() { }
    // For creating new department with just a name
    public Department(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Department(
        Guid id,
        string name , 
        IEnumerable<Guid> patientIds, 
        IEnumerable<Guid> doctorIds)
    {
        Id = id;
        Name = name;
        _patientIds.AddRange(patientIds);
        _doctorIds.AddRange(doctorIds);
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void AdmitPatient(Guid patientId)
    {
        if(_patientIds.Contains(patientId)) 
            throw new InvalidOperationException($"Patient with ID {patientId} is already admitted in department '{Name}'.");

        _patientIds.Add(patientId);
    }

    // في حال انتقل لقسم تاني او قبل التخريج 
    // بينما فعل التخريج الاساسسي عند المريض 
    public void RemovePatient(Guid patientId)
    {
        if(!_patientIds.Contains(patientId)) 
            throw new InvalidOperationException($"Patient with ID {patientId} is not admitted in department '{Name}'.");

        _patientIds.Remove(patientId);
    }

    public void AssignDoctor(Guid doctorId)
    {
        if (_doctorIds.Contains(doctorId))
            throw new InvalidOperationException(
                $"Doctor {doctorId} is already assigned to department '{Name}'.");
        _doctorIds.Add(doctorId);
    }

    public void RemoveDoctor(Guid doctorId)
    {
        if (!_doctorIds.Contains(doctorId))
            throw new InvalidOperationException(
                $"Doctor {doctorId} is not in department '{Name}'.");
        _doctorIds.Remove(doctorId);
    }

    internal void LoadPatientIds(IEnumerable<Guid> ids)
        => _patientIds.AddRange(ids);

    internal void LoadDoctorIds(IEnumerable<Guid> ids)
        => _doctorIds.AddRange(ids);

}
