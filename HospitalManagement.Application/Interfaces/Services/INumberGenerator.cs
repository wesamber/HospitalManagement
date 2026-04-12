namespace HospitalManagement.Application.Interfaces.Services;

public interface INumberGenerator
{
    string GenerateUniqueNumber(string prefix);
}
