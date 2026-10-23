using HR2UpdateTool.Application.Interface;
using HR2UpdateTool.Domain.Repositories;
using HR2UpdateTool.Infrastructure.Repositories;
using HR2UpdateTool.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ISerialService, SerialService>();
            services.AddSingleton<ISerialMessenger, SerialMessenger>();
            services.AddSingleton<IFirmwareUpdateRepository, FirmwareUpdateRepository>();

            return services;
        }
    }
}
