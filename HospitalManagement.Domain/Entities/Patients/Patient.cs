using HospitalManagement.Domain.Contracts;
using HospitalManagement.Domain.Entities.Treatments;

namespace HospitalManagement.Domain.Entities.Patients;

public abstract class Patient : IEntity
{
    public Guid Id { get; private set; }
    public string PatientNumber {  get; protected set; } = string.Empty;
    public string Name { get; protected set; } = string.Empty;
    public DateOnly? DateOfBirth { get;protected set; }
    public string? Address { get;protected set; }
    public string? PhoneNumber { get;protected set; }
    public string? Email { get;protected set; }
   
    private readonly List<Guid> _externalTreatmentIds = new();
    public IReadOnlyList<Guid> ExternalTreatmentIds => _externalTreatmentIds.AsReadOnly();

    // constructor for loading existing patients
    protected Patient() { }
    protected Patient(Guid id,
        string patientNumber,
        string name,
        DateOnly? dateOfBirth,
        string? address,
        string? phone,
        string? email,
        IEnumerable<Guid>? treatmentIds=null)
    {
        Id = id;
        PatientNumber = patientNumber;
        Name = name;
        DateOfBirth = dateOfBirth;
        Address = address;
        PhoneNumber = phone;
        Email = email;

        if (treatmentIds != null)
        {
            _externalTreatmentIds.AddRange(treatmentIds);
        }
    }

    // constructor for creating new patients
    protected Patient(
        string patientNumber,
        string name,
        DateOnly? dateOfBirth,
        string? address,
        string? phone,
        string? email):
        this(Guid.NewGuid(), patientNumber, name,
            dateOfBirth, address, phone, email , null)
    {
        // باني الانشاء عم يرث من باني اعادة التحميل مشان ما ولدد guid وكبو
    }

    public void AddExternalTreatment(TreatmentExternal treatment)
    {
        if (treatment != null && !_externalTreatmentIds.Contains(treatment.Id))
            _externalTreatmentIds.Add(treatment.Id);
    }
}
