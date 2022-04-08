using GreenPipes;
using MassTransit;
using MassTransit.Registration;
using MassTransit.Transactions;

namespace ERokytne.Api.Infrastructure.Extensions;

public static class MassTransitExtension
{
    public static void AddMasstransitConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
            {
                configure.AddBus(provider => CreateBusControl(provider, configuration));

                configure.AddMessageScheduler(new Uri("queue:SchedulerService:Commands"));
                configure.AddTransactionalEnlistmentBus();
            })
            .AddMassTransitHostedService();
    }
    
    private static IBusControl CreateBusControl(IConfigurationServiceProvider provider, 
            IConfiguration configuration)
    {
    
        var transport = configuration.GetValue<string>("MassTransit:Transport")?.ToUpper();
    
        var busControl = transport switch
        {
            "RABBITMQ" => Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(configuration.GetValue<string>("MassTransit:HostSettings:Host"), "/", configurator =>
                {
                    configurator.Username(configuration.GetValue<string>("MassTransit:HostSettings:Username"));
                    configurator.Password(configuration.GetValue<string>("MassTransit:HostSettings:Password"));
                });
    
                ConfigureBusFactoryConfigurator(configure, provider, configuration);
            }),
            _ => Bus.Factory.CreateUsingInMemory(configure =>
            {
                ConfigureBusFactoryConfigurator(configure, provider, configuration);
            })
        };
    
        foreach (var consumeObserver in provider.GetServices<IConsumeObserver>())
        {
            busControl.ConnectConsumeObserver(consumeObserver);
        }
    
        return busControl;
    }
    
    private static void ConfigureBusFactoryConfigurator(IReceiveConfigurator factoryConfigurator, IServiceProvider provider, IConfiguration configuration)
    {
        factoryConfigurator.ReceiveEndpoint(
            configuration.GetValue<string>("General:ServiceName"),
            configurator =>
            {
                configurator.UseConcurrencyLimit(1);
                configurator.UseRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });
                
            });                  
    }
}