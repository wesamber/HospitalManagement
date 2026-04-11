using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Doctors;

namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentInternal : Treatment , IEntity
{
    public DateTime DateDischarge { get;private set; }
    public Guid DepartmentId { get;private set; }
    public Guid PatientId { get;private set; } 

    private readonly List<DoctorTreatment> _doctors = new();
    public IReadOnlyList<DoctorTreatment> Doctors => _doctors.AsReadOnly();
    private TreatmentInternal() { }

    // for loading
    public TreatmentInternal(
        Guid id , DateTime datestart , decimal cost,
        DateTime dateDicharge , Guid departmentId, Guid patientId)
        : base(id, datestart, cost)
    {
        DateDischarge = dateDicharge;
        DepartmentId = departmentId;
        PatientId = patientId;
    }

    // for creating
    public TreatmentInternal(
        DateTime datestart, decimal cost,
        DateTime dateDicharge, Guid departmentId, Guid patientId)
        : base(datestart, cost)
    {
        DateDischarge = dateDicharge;
        DepartmentId = departmentId;
        PatientId = patientId;
    }

    public void AddDoctor(DoctorTreatment doctorTreatment)
    {
        if (doctorTreatment == null)
            throw new ArgumentNullException(nameof(doctorTreatment));

        if(_doctors.Any(d => d.DoctorId == doctorTreatment.DoctorId))
            throw new InvalidOperationException("Doctor is already assigned to this treatment.");

        if(doctorTreatment.TreatmentId != Id)
            throw new InvalidOperationException("DoctorTreatment's TreatmentId does not match this Treatment's Id.");

        _doctors.Add(doctorTreatment);
    }
}
