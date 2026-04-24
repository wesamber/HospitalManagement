using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models;

public class SalaryRecordJsonModel
{
    public DateOnly RecordedAt { get; set; }
    public decimal Amount { get; set; }
}
