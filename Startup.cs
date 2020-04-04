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
using Microsoft.Extensions.Logging;

namespace kube_consul_registrator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                var address = Configuration.GetValue<string>("Consul:Address");
                consulConfig.Address = new Uri("http://" + address);
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
    }
}
