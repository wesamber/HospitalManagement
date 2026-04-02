using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models
{
    public class InternalPatient : Patient
    {
        public string AdmittedDepartment { get; set; }

        public List<TreatmentInternal> InternalTreatments { get; set; } = new();
        public List<TreatmentExternal> ExternalTreatments { get; set; } = new();

        public bool IsDischarged { get; set; } = false;
        public DateTime? DischargeDate { get; set; }

        public InternalPatient() { }

        public InternalPatient(int id, string name, DateOnly DateOfBirth, string address, string number,string email, string department )
            : base(id, name, DateOfBirth, address,number,email)
        {
            AdmittedDepartment = department;
        }

        public void AddInternalTreatment(TreatmentInternal treatment)
        {
            InternalTreatments.Add(treatment);
        }

        public void AddExternalTreatment(TreatmentExternal treatment)
        {
            ExternalTreatments.Add(treatment);
        }

        public void Discharge()
        {
            IsDischarged = true;
            DischargeDate = DateTime.Now;
            Console.WriteLine($"Patient {Name} discharged on {DischargeDate:yyyy-MM-dd}.");
        }
        public bool IsDischarge() => DischargeDate.HasValue;

    }

}
