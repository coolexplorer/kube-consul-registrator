namespace kube_consul_registrator.Configurations
{
    public interface IConsulConfig
    {
        string Name { get; set; }
        int port { get; set; }
        string Address { get; set; }

    }
}