using System.Diagnostics.CodeAnalysis;

namespace kube_consul_registrator.Const
{
    [ExcludeFromCodeCoverage]
    public static class Annotations
    {
        public const string EABLED_ANNOTATION = "consul-registrator/enabled";
        public const string SERVICE_ID_ANNOTATION = "consul-registrator/service-id";
        public const string SERVICE_NAME_ANNOTATION = "consul-registrator/service-name";
        public const string SERVICE_PORT_ANNOTATION = "consul-registrator/service-port";
        public const string SERVICE_METADATA_ANNOTATION = "consul-registrator/service-meta-";
    }
}