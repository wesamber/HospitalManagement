using HospitalManagement.Application.Common.Enums;
using HospitalManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Services;

public class NumberGenerator : INumberGenerator
{
    public string GenerateUniqueNumber(NumberPerfix prefix)
    {
        var prefixString = prefix switch
        {
            NumberPerfix.Doctor => "DOC",
            NumberPerfix.Patient => "PAT",
            NumberPerfix.Treatment => "TRT",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{prefixString}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
}
