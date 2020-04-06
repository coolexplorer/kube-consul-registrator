using System.Linq;
using System.Reflection;
using System;
using kube_consul_registrator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using kube_consul_registrator.Extensions;
using kube_consul_registrator.Repositories;
using Consul;
using AutoMapper;
using kube_consul_registrator.Configurations;

namespace kube_consul_registrator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            GetEnvironmentVaraibles();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCronJob<ConsulRegisterCronJob>(c => 
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/10 * * * * *";
            });
            services.AddSingleton<IKubernetesRepository, KubernetesRepository>();
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig => 
            {
                consulConfig.Address = new Uri("http://" + EnvironmentVariables.ConsulAddress);
            }));
            services.AddSingleton<IConsulRepository, ConsulRepository>();
            services.AddAutoMapper(Assembly.GetAssembly(this.GetType()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void GetEnvironmentVaraibles()
        {
            var allowedNamespaces = Environment.GetEnvironmentVariable("KUBE_ALLOWED_NAMESPACES");

            if (allowedNamespaces == null)
            {
                EnvironmentVariables.AllowedNamespaces = Configuration.GetValue<string>("Kube:AllowedNameSpaces").Split(",");
            } else 
            {
                EnvironmentVariables.AllowedNamespaces = allowedNamespaces.Split(",");
            }

            EnvironmentVariables.ConsulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");

            if (EnvironmentVariables.ConsulAddress == null)
            {
                EnvironmentVariables.ConsulAddress = Configuration.GetValue<string>("Consul:Address");
            } 

            Console.WriteLine($"CONSUL_ADDRESS: {EnvironmentVariables.ConsulAddress}");
            Console.WriteLine("KUBE_ALLOWED_NAMESPACES: [{0}]", string.Join(",", EnvironmentVariables.AllowedNamespaces));
        }
    }
}
