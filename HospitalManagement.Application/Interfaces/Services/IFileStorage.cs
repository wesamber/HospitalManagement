namespace HospitalManagement.Application.Interfaces.Services;

public interface IFileStorage
{
    Task<string> ReadAsync(string path);
    Task WriteAsync(string path , string content);
    Task AppendLineAsync(string path, string content);
}
