using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using kube_consul_registrator.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator.Services
{
    public abstract class CronJobService : IHostedService, IDisposable
    {
        private CronExpression _expression;
        private System.Timers.Timer _timer;

        private readonly TimeZoneInfo _timeZoneInfo;

        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo)
        {
            _expression = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
            _timeZoneInfo = timeZoneInfo;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await GetScheduledJob(cancellationToken);
        }

        protected virtual async Task GetScheduledJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);

            if (next.HasValue)
            {
                var delayTime = next.Value - DateTimeOffset.Now;
                
                _timer = new System.Timers.Timer(delayTime.TotalMilliseconds);

                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await RunWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await GetScheduledJob(cancellationToken);
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task RunWork(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}