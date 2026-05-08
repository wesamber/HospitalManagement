namespace HospitalManagement.Domain.Entities.Treatments;

public class TreatmentExternal : Treatment
{
    public string ClinicNumber { get;private set; } = string.Empty;
    public string DoctorName { get;private set; } = string.Empty;

    public TreatmentExternal() { }

    // constructor for loading
    public TreatmentExternal(
        Guid id, string numberTreatment,string name, DateTime startDate, decimal cost,
        string clinicNumber, string doctorName,
        Guid patientId)
        : base(id,numberTreatment,name, startDate, cost , patientId)
    {
        ClinicNumber = clinicNumber;
        DoctorName = doctorName;
    }

    //constructor for creating
    public TreatmentExternal(
        string numberTreatment,string name ,DateTime startDate, decimal cost,
        string clinicNumber, string doctorName,
        Guid patientId)
        : base(numberTreatment,name ,startDate, cost , patientId)
    {
        ClinicNumber = clinicNumber;
        DoctorName = doctorName;
    }
}
