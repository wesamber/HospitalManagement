using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Infrastructure.Serialization;
using System.Text;

namespace HospitalManagement.Infrastructure.Storage;

public class FileStorage : IFileStorage
{
    public async Task AppendLineAsync(string path, string content)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.AppendAllTextAsync(path, content + Environment.NewLine);
    }


    public async Task<string> ReadAsync(string path)
    {
        if (!File.Exists(path))
            return string.Empty;

        return await File.ReadAllTextAsync(path);
    }

    public async Task WriteAsync(string path, string content)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(path, content);
    }
}
