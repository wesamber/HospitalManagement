namespace HospitalManagement.Application.Interfaces.Services;

public interface ISerializer
{
    string Serialize<T>(T data , bool indented = false);
    T Deserialize<T>(string content);
}
