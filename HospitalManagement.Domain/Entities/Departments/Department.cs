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

    private Department() { }
    // For creating new department with just a name
    public Department(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Department(
        string name , 
        IEnumerable<InternalPatient> patients, 
        IEnumerable<Doctor> doctors)
    {
        Id = Guid.NewGuid();
        Name = name;
        _patientIds.AddRange(patients.Select(p => p.Id));
        _doctorIds.AddRange(doctors.Select(d => d.Id));
    }

    public void AdmitPatient(Guid patientId)
    {
        if(!_patientIds.Contains(patientId))
            _patientIds.Add(patientId);
    }

    //public void DischargePatient(Guid patientId , DateTime date)
    //{
    //    var patient = _patients.FirstOrDefault(p => p.Id == patientId);
    //    if(patient is null)
    //        throw new InvalidOperationException($"Patient with ID {patientId} not found in department '{Name}'.");
    //    patient.Discharge(date);
    //}

    //public int CountPatientsInPeriod(DateTime from, DateTime to)
    //     => _patients.Count(p =>
    //    p.InternalTreatments.Any(t => t.DateStart >= from && t.DateStart     <= to));
    
}
