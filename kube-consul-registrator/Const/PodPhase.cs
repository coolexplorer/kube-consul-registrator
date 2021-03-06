using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Const
{
    [ExcludeFromCodeCoverage]
    public static class PodPhase
    {
        public static string PENDING = "Pending";
        public static string RUNNING = "Running";
        public static string SUCCEEDED = "Succeeded";
        public static string FAILED = "Failed";
        public static string UNKNOWN = "Unknown";
    }
}