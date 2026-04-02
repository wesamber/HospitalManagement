using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models
{
    public class ContractedRole : DoctorRole
    {
        public decimal Percent { get; set; } = 0.5m; // 50%
        public List<Treatment> TreatmentsDone { get; set; } = new();

        public ContractedRole(DateTime startDate)
        {
            StartDate = startDate;
        }

        public override decimal CalculateSalary()
        {
            decimal total = TreatmentsDone.Sum(t => t.Cost);
            return total * Percent;
        }
    }
}
