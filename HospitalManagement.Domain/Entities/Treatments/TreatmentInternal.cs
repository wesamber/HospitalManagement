using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Patients;
using System.Text.Json.Serialization;

namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentInternal : Treatment
{
    public DateTime DateDicharge { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PatientId { get; set; } 

    private readonly List<DoctorTreatment> _doctors = new();
    public IReadOnlyList<DoctorTreatment> Doctors => _doctors.AsReadOnly();
    private TreatmentInternal() { }

    // for loading
    public TreatmentInternal(
        Guid id , DateTime datestart , decimal cost,
        DateTime dateDicharge , Guid departmentId, Guid patientId)
        : base(id, datestart, cost)
    {
        DateDicharge = dateDicharge;
        DepartmentId = departmentId;
        PatientId = patientId;
    }

    // for creating
    public TreatmentInternal(
        DateTime datestart, decimal cost,
        DateTime dateDicharge, Guid departmentId, Guid patientId)
        : base(datestart, cost)
    {
        DateDicharge = dateDicharge;
        DepartmentId = departmentId;
        PatientId = patientId;
    }

    public void AddDoctor(DoctorTreatment doctorTreatment)
    {
        if (doctorTreatment == null)
            throw new ArgumentNullException(nameof(doctorTreatment));

        if(_doctors.Any(d => d.DoctorId == doctorTreatment.DoctorId))
            throw new InvalidOperationException("Doctor is already assigned to this treatment.");

        if(doctorTreatment.TreatmentId != TreatmentId)
            throw new InvalidOperationException("DoctorTreatment's TreatmentId does not match this Treatment's Id.");

        _doctors.Add(doctorTreatment);
    }
}
