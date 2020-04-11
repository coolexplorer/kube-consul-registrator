using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class EnvironmentVariables
    {
        public static string ConsulAddress = null;
        public static string[] AllowedNamespaces = null;
    }
}