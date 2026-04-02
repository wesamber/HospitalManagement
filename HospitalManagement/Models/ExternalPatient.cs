using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models
{
    public class ExternalPatient : Patient
    {
        public bool IsAdmitted { get; set; } = false;
        public DateTime? AdmissionDate { get; set; }

        public List<TreatmentExternal> ExternalTreatments { get; set; } = new();

        public ExternalPatient() { }

        public ExternalPatient(int id, string name, DateOnly DateOfBirth, string address, string number, string email)
            : base(id, name, DateOfBirth, address, number, email)
        {
            AdmissionDate = null;
        }

        public void AddExternalTreatment(TreatmentExternal treatment)
        {
            ExternalTreatments.Add(treatment);
        }

        public InternalPatient Admit(string department)
        {
            IsAdmitted = true;
            AdmissionDate = DateTime.Now;

            // إنشاء مريض داخلي بنفس البيانات
            var internalPatient = new InternalPatient(
                PatientId,
                Name,
                DateOfBirth,
                Address,
                PhoneNumber,
                Email,
                department
            );

            // نقل العلاجات الخارجية
            foreach (var t in ExternalTreatments)
                internalPatient.AddExternalTreatment(t);

            return internalPatient;
        }
    }

}
