using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Services;
using HospitalManagement.Infrastructure.Configuration;
using HospitalManagement.Infrastructure.Persistence.Ado;
using HospitalManagement.Infrastructure.Persistence.Json;
using HospitalManagement.Infrastructure.Serialization;
using HospitalManagement.Infrastructure.Services;
using HospitalManagement.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace HospitalManagement.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // read setting from appsettings.json
        var storageOption = new FileStorageOption();
        configuration.GetSection(FileStorageOption.SectionName).Bind(storageOption);

        // register primary services
        services.AddSingleton<IFileStorage, FileStorage>();
        services.AddSingleton<ISerializer>(new Serializer());
        services.AddScoped<INumberGenerator, NumberGenerator>();

        if(storageOption.ActiveProvider == "Json")
        {
            services.AddScoped<IDoctorRepository>(sp =>
            new JsonDoctorRepository(
                Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.DoctorsSnapshot),
                Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.DoctorsLog),
                sp.GetRequiredService<IFileStorage>(),
                sp.GetRequiredService<ISerializer>()
            ));

            services.AddScoped<IDepartmentRepository>(sp =>
                new JsonDepartmentRepository(
                    Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.DepartmentsSnapshot),
                    Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.DepartmentsLog),
                    sp.GetRequiredService<IFileStorage>(),
                    sp.GetRequiredService<ISerializer>()
                ));

            services.AddScoped<IPatientRepository>(sp =>
                new JsonPatientRepository(
                    Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.PatientsSnapshot),
                    Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.PatientsLog),
                    sp.GetRequiredService<IFileStorage>(),
                    sp.GetRequiredService<ISerializer>()
                ));

            services.AddScoped<ITreatmentRepository>(sp =>
                new JsonTreatmentRepository(
                    Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.TreatmentsSnapshot),
                    Path.Combine(AppContext.BaseDirectory, storageOption.JsonPaths.TreatmentsLog),
                    sp.GetRequiredService<IFileStorage>(),
                    sp.GetRequiredService<ISerializer>()
                ));

            services.AddScoped<ISystemConfigRepository>(sp =>
                new JsonSystemConfigRepository(
                filePath: "Data/system-config.json",
                fileStorage: sp.GetRequiredService<IFileStorage>(),
                serializer: sp.GetRequiredService<ISerializer>()
                 ));

            services.AddScoped<ISystemConfigService, SystemConfigService>();
        }
        else if(storageOption.ActiveProvider == "Database")
        {
            //هون الحقن الريبو تبعات الداتا بيز
            var connectionString = storageOption.ConnectionStrings.DefaultConnection;

            services.AddScoped<ISqlConnectionFactory>(sp =>
                new SqlConnectionFactory(connectionString));

            services.AddScoped<ISystemConfigRepository, AdoSystemConfigRepository>();
            services.AddScoped<IDoctorRepository, AdoDoctorRepository>();
            services.AddScoped<IDepartmentRepository, AdoDepartmentRepository>();
            services.AddScoped<IPatientRepository, AdoPatientRepository>();
            services.AddScoped<ITreatmentRepository, AdoTreatmentRepository>();
        }

        return services;
    }
}
