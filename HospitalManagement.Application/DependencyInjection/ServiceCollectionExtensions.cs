using AutoMapper;
using FluentValidation;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Application.Mappers.Doctors;
using HospitalManagement.Application.Services;
using HospitalManagement.Application.Services.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HospitalManagement.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // register application services
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<ITreatmentService, TreatmentService>();

        services.AddHostedService<TraineeSalaryUpdateWorker>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}
