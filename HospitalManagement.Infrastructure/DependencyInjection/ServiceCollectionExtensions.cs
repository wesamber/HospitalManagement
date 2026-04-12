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
        services.AddSingleton<ISerializer>(new Serializer());

        services.AddSingleton<IDoctorRepository>(sp =>
            new JsonDoctorRepository(
                Path.Combine(AppContext.BaseDirectory, "Data", "doctors.snapshot.json"),
                Path.Combine(AppContext.BaseDirectory, "Data", "doctors.log.jsonl"),
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));

        services.AddSingleton<IPatientRepository>(sp =>
            new JsonPatientRepository(
                Path.Combine(AppContext.BaseDirectory, "Data", "doctors.snapshot.json"),
                Path.Combine(AppContext.BaseDirectory, "Data", "doctors.log.jsonl"),
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));

        services.AddSingleton<ITreatmentRepository>(sp =>
            new JsonTreatmentRepository(
                Path.Combine(AppContext.BaseDirectory, "Data", "treatments.snapshot.json"),
                Path.Combine(AppContext.BaseDirectory, "Data", "treatments.log.jsonl"),
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));
        return services;
    }
}
