using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Domain.Entities.Patients;

public class InternalPatient : Patient , IEntity
{
    public bool IsDischarged { get;private set; } = false;
    public DateTime? DischargeDate { get;private set; }
    public  Guid DepartmentId { get;private set; } // القسم يلي انقبل فيه اجباري

    private readonly List<Guid> _internalTreatments = new();
    public IReadOnlyList<Guid> InternalTreatments => _internalTreatments.AsReadOnly();
    protected InternalPatient() { }

    // constructor for creating
    public InternalPatient(
        string patientNumber,string name,
        DateOnly? dateOfBirth,
        string? address,string? phone,
        string? email, Guid departmentId)
        : base(patientNumber, name, dateOfBirth, address, phone, email)
    {
        DepartmentId = departmentId;
    }

    // constructor for loading
    public InternalPatient(
        Guid id,string patientNumber,string name,
        DateOnly? dateOfBirth,string? address,
        string? phone, string? email,
        Guid departmentId , 
        IEnumerable<Guid>? internalTreatments, 
        IEnumerable<Guid>? externalTreatments)
        : base(id, patientNumber, name, dateOfBirth, address, phone, email, 
              externalTreatments)
    {
        DepartmentId = departmentId;

        if(internalTreatments != null) 
            _internalTreatments.AddRange(internalTreatments);
    }

    public void AddInternalTreatment(TreatmentInternal treatment)
    {
        if (treatment == null)
            throw new ArgumentNullException(nameof(treatment));

        _internalTreatments.Add(treatment.Id);
    }


    public void Discharge(DateTime dischargeDate)
    {
        if (IsDischarged) 
            throw new InvalidOperationException("Patient is already discharged.");

        IsDischarged = true;
        DischargeDate = dischargeDate; 
    }

}
