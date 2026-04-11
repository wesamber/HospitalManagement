namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentExternal : Treatment
{
    public string ClinicNumber { get;private set; } = string.Empty;
    public Guid DoctorId { get;private set; }
    public Guid PatientId { get;private set; }

    private TreatmentExternal() { }

    // constructor for loading
    public TreatmentExternal(
        Guid id, DateTime date,
        string notes, decimal cost,
        string clinicNumber, Guid doctorId,
        Guid patientId)
        : base(id, date, cost)
    {
        ClinicNumber = clinicNumber;
        DoctorId = doctorId;
        PatientId = patientId;
    }

    //constructor for creating
    public TreatmentExternal(
        DateTime date, decimal cost,
        string clinicNumber, Guid doctorId ,
        Guid patientId)
        : base(date, cost)
    {
        ClinicNumber = clinicNumber;
        DoctorId = doctorId;
        PatientId = patientId;
    }
}
