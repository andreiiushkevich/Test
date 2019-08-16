using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.Common.Configuration;
using Test.Common.Dtos;
using Test.Receiver.Hubs;

namespace Test.Facade.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSignalR();

            services.AddSingleton(Configuration.GetSection("RabbitMqValues").Get<RabbitMqValues>());

            services.AddMassTransit(conf =>
            {
                conf.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var rabbitMqValues = provider.GetService<RabbitMqValues>();

                    cfg.Host(new Uri(rabbitMqValues.HostAddress), hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqValues.Username);
                        hostConfigurator.Password(rabbitMqValues.Password);
                    });

                    EndpointConvention.Map<ContractDto>(new Uri(rabbitMqValues.DestinationAddress));
                }));
            });

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<ContractDto>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IBusControl busControl)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            busControl.StartAsync().Wait();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSignalR(config => { config.MapHub<ContractHub>("/contracthub"); });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}