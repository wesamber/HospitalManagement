using HospitalManagement.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services.BackgroundServices;

public class TraineeSalaryUpdateWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public TraineeSalaryUpdateWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var doctorService = scope.ServiceProvider.GetRequiredService<IDoctorService>();

                await doctorService.UpdateTraineeSalariesAsync();
                
            }

             await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
