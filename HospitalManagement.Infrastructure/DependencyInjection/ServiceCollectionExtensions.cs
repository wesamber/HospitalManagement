using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Infrastructure.Persistence.Json;
using HospitalManagement.Infrastructure.Serialization;
using HospitalManagement.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace HospitalManagement.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileStorage, FileStorage>();

        // Serializer + Polymorphism
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            TypeInfoResolver = new JsonTypeInfoResolver()
        };
        services.AddSingleton<ISerializer>(new Serializer());

        services.AddScoped<IDoctorRepository>(sp =>
            new JsonDoctorRepository(
                "Data/doctors.snapshot.json",
                "Data/doctors.log.jsonl",
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));

        services.AddScoped<IPatientRepository>(sp =>
            new JsonPatientRepository(
                "Data/patients.snapshot.json",
                "Data/patients.log.jsonl",
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));

        services.AddScoped<ITreatmentRepository>(sp =>
            new JsonTreatmentRepository(
                "Data/treatments.snapshot.json",
                "Data/treatments.log.jsonl",
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));
        return services;
    }
}
