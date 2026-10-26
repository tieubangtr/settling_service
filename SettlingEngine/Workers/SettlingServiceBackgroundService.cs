using DapperLib.LinQToSql.Core;
using Microsoft.Extensions.Hosting;
using SettlingEngine.MemoryDatas;

namespace SettlingEngine.Workers
{
    internal class SettlingServiceBackgroundService : BackgroundService
    {
        private readonly IMemoryInstance _memory;

        public SettlingServiceBackgroundService(IMemoryInstance memory)
        {
            _memory = memory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await ReloadMemory(stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }
        }

        private async Task ReloadMemory(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime startTime = DateTime.Now;
                try
                {
                    await _memory.Reload();
                    Logger.log.Info($"SettlingServiceBackgroundService ReloadMemory completed in {(DateTime.Now - startTime).TotalMilliseconds} ms.");
                }
                catch (Exception ex)
                {
                    Logger.log.Error(ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
