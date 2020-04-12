using System;
using System.Diagnostics.CodeAnalysis;
using k8s;
using kube_consul_registrator.Configurations;
using kube_consul_registrator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kube_consul_registrator.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentException(nameof(options), @"Providing Schedule configurations is essential.");
            }

            var config = new ScheduleConfig<T>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentException(nameof(ScheduleConfig<T>.CronExpression), @"Empty cron expression is not allowed");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            
            return services;
        }
    }
}