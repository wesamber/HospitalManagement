using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Interfaces;

public interface INumberGenerator
{
    string GenerateUniqueNumber(string prefix);
}
