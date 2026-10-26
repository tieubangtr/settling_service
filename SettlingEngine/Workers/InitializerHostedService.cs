using Microsoft.Extensions.Hosting;
using SettlingEngine.MemoryDatas;
using SettlingEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlingEngine.Workers
{
    internal class InitializerHostedService : IHostedService
    {
        private readonly IMemoryInstance _memoryInstance;

        public InitializerHostedService(IMemoryInstance memoryInstance)
        {
            _memoryInstance = memoryInstance;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _memoryInstance.Initialize();
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
