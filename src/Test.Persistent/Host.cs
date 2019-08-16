using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using GreenPipes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test.Common.Configuration;
using Test.Persistent.Context;
using Test.Persistent.Services;

namespace Test.Processor
{
    internal class Host
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                    config.AddJsonFile(
                        "rabbitmq.json", false, false);
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile(
                        "appsettings.json", false, false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(hostContext.Configuration.GetSection("RabbitMqValues").Get<RabbitMqValues>());
                    services.AddDbContext<TestContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                    services.AddTransient<DbContext>(provider => provider.GetService<TestContext>());

                    services.AddSingleton<IContractsRepository, ContractsRepository>();
                    services.AddSingleton<IOrganizationsRepository, OrganizationsRepository>();
                    services.AddSingleton<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<ContractConsumer>();

                    services.AddMassTransit(conf =>
                    {
                        // add the consumer to the container
                        conf.AddConsumer<ContractConsumer>();
                    });

                    services.AddMassTransit(configurator =>
                    {
                        configurator.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var rabbitMqValues = provider.GetService<RabbitMqValues>();

                            var host = cfg.Host(new Uri(rabbitMqValues.HostAddress), hostConfigurator =>
                            {
                                hostConfigurator.Username(rabbitMqValues.Username);
                                hostConfigurator.Password(rabbitMqValues.Password);
                            });

                            cfg.ReceiveEndpoint(host, rabbitMqValues.QueueName, endpointConfigurator =>
                            {
                                endpointConfigurator.PrefetchCount = 16;
                                endpointConfigurator.UseMessageRetry(x => x.Interval(2, 100));

                                endpointConfigurator.Consumer<ContractConsumer>(provider);
                            });
                        }));
                    });

                    services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
                    services.AddSingleton<ISendEndpointProvider>(provider =>
                        provider.GetRequiredService<IBusControl>());
                    services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
                    services.AddHostedService<BootstrapBus>();

                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                });

            await builder.RunConsoleAsync();
        }
    }
}