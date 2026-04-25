using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Services;
using HospitalManagement.Infrastructure.Configuration;
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
            // هون الحقن الريبو تبعات الداتا بيز
            // 4. هنا نضع ريبو الأدو (ADO.NET)
            //// سنمرر الـ ConnectionString من ملف الإعدادات
            //var connectionString = storageOptions.ConnectionStrings.DefaultConnection;

            //services.AddScoped<IDoctorRepository>(sp => new SqlDoctorRepository(connectionString));
            //services.AddScoped<IPatientRepository>(sp => new SqlPatientRepository(connectionString));
            //services.AddScoped<ITreatmentRepository>(sp => new SqlTreatmentRepository(connectionString));
        }

        // هون كمان بدي ضيف الانترفيسات تبع الريبو تبع  الادو
        return services;
    }
}
