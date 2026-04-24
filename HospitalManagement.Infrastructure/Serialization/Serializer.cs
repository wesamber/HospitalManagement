using HospitalManagement.Application.Interfaces.Services;
using System.Text.Json;

namespace HospitalManagement.Infrastructure.Serialization;

public class Serializer : ISerializer
{
    private readonly JsonSerializerOptions _indented;
    private readonly JsonSerializerOptions _compact;

    public Serializer()
    {
        _indented = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = new JsonTypeInfoResolver()
        };

        _compact = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = new JsonTypeInfoResolver()
        };
    }

    public string Serialize<T>(T data, bool indented = false)
    {
        return JsonSerializer.Serialize(data, indented ? _indented : _compact);
    }

    public T Deserialize<T>(string content)
    {
        return JsonSerializer.Deserialize<T>(content, _compact)!;
    }
}
