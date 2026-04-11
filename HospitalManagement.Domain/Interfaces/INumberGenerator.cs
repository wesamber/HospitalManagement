namespace HospitalManagement.Domain.Interfaces;

public interface INumberGenerator
{
    string GenerateUniqueNumber(string prefix);
}
