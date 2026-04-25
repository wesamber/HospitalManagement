using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models.Doctors;

public class SalaryRecordJsonModel
{
    public DateTime RecordedAt { get; set; }
    public decimal Amount { get; set; }
}
