using System;
using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Configurations
{
    [ExcludeFromCodeCoverage]
    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}