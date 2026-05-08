using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Doctors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentInternal : Treatment , IEntity
{
    public DateTime? DateDischarge { get;private set; }
    public Guid DepartmentId { get;private set; }

    private readonly List<DoctorTreatment> _doctors = new();
    public IReadOnlyList<DoctorTreatment> Doctors => _doctors.AsReadOnly();
    public TreatmentInternal() { }

    // for loading
    public TreatmentInternal(
        Guid id, string numberTreatment, string name ,DateTime datestart, decimal cost,
        DateTime dateDicharge , Guid departmentId, Guid patientId)
        : base(id, numberTreatment, name, datestart, cost, patientId)
    {
        DateDischarge = dateDicharge;
        DepartmentId = departmentId;
    }

    // for creating
    public TreatmentInternal(
        string numberTreatment, string name, DateTime datestart, decimal cost,
        DateTime dateDicharge, Guid departmentId, Guid patientId)
        : base(numberTreatment, name, datestart, cost , patientId)
    {
        DateDischarge = dateDicharge;
        DepartmentId = departmentId;
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

    public void Discharge(DateTime dischargeDate)
    {
        if (DateDischarge != null)
            throw new InvalidOperationException("Already discharged.");

        if (dischargeDate < StartDate   )
            throw new InvalidOperationException("Discharge date cannot be before start date.");

        DateDischarge = dischargeDate;
    }
}
