using HospitalManagement.Application.Interfaces.Services;
using System.Text.Json;

namespace HospitalManagement.Infrastructure.Serialization;

public class Serializer : ISerializer
{
    private readonly JsonSerializerOptions _options;
    
    public Serializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true, // تنسيق
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = new JsonTypeInfoResolver()
        };
    }

    public string Serialize<T>(T data)
    {
        return JsonSerializer.Serialize(data, _options);
    }

    public T Deserialize<T>(string content)
    {
        return JsonSerializer.Deserialize<T>(content , _options)!;
    }
}
