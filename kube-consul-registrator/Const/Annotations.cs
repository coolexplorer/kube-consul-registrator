using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Const
{
    [ExcludeFromCodeCoverage]
    public static class Annotations
    {
        public const string EABLED = "consul-registrator/enabled";
        public const string SERVICE_ID = "consul-registrator/service-id";
        public const string SERVICE_NAME = "consul-registrator/service-name";
        public const string SERVICE_PORT = "consul-registrator/service-port";
        public const string SERVICE_TAG = "consul-registrator/service-tag"; 
        public const string SERVICE_METADATA = "consul-registrator/service-meta-";
    }
}