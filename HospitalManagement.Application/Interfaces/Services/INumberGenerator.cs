using HospitalManagement.Application.Common.Enums;

namespace HospitalManagement.Application.Interfaces.Services;

public interface INumberGenerator
{
    string GenerateUniqueNumber(NumberPerfix prefix);
}
