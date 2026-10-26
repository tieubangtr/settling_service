using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SettlingEngine.DataAccesses;
using SettlingEngine.Helpers;
using SettlingEngine.MemoryDatas;
using SettlingEngine.Services;
using SettlingEngine.Workers;

namespace SettlingEngine
{
    public class SettlingServiceStartup
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOracleHelper, OracleHelper>();
            services.AddScoped<IDataAccess, DataAccess>();
            services.AddSingleton<IMemoryInstance, MemoryInstance>();
            services.AddScoped<ISettlingService, SettlingService>();

            services.AddHostedService<InitializerHostedService>();

            services.AddHostedService<SettlingServiceBackgroundService>();
        }
    }
}
