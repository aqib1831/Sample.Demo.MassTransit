using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit.Definition;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Contracts;
using MassTransit.Courier.Contracts;
using MassTransit.Services.Consumers;
using MassTransit.Services.CourierActivities;
using MassTransit.Services.StateMachine;
using MongoDB.Driver;
using MassTransit.MongoDbIntegration.MessageData;
using MassTransit.Services;

namespace WebApplication1
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
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.UseAllOfToExtendReferenceSchemas();
            });

            services.AddControllers();

            BootstrapMassTransit(services);
        }

        private void BootstrapMassTransit(IServiceCollection _services)
        {
            _services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            _services.AddMassTransit(cfg =>
            {
                BootstrapStateMachine(cfg);
                cfg.UsingInMemory(ConfigureInMemoryBus);

            });

            _services.AddMassTransitHostedService();

        }

        private static void BootstrapStateMachine(IRegistrationConfigurator serviceBusConfigurator)
        {
            serviceBusConfigurator.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
            serviceBusConfigurator.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();
            //serviceBusConfigurator.AddConsumersFromNamespaceContaining<SeriesFaultConsumer>();


            serviceBusConfigurator.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = "mongodb://localhost:27017/";
                    r.DatabaseName = "Demo";
                    r.CollectionName = "OrderStates";
                });

            serviceBusConfigurator.AddRequestClient<SubmitOrderPayload>();
            serviceBusConfigurator.AddRequestClient<CheckOrder>();

        }
        private static void ConfigureInMemoryBus(IBusRegistrationContext context, IInMemoryBusFactoryConfigurator configurator)
        {
            //configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "demoaq"));
            //configurator.UseMessageScheduler(new Uri("queue:quartz"));

            //configurator.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipBatchEventConsumer>(), e =>
            //{
            //    e.PrefetchCount = 20;

            //    e.Batch<RoutingSlipCompleted>(b =>
            //    {
            //        b.MessageLimit = 10;
            //        b.TimeLimit = TimeSpan.FromSeconds(5);

            //        b.Consumer<RoutingSlipBatchEventConsumer, RoutingSlipCompleted>(context);
            //    });
            //});

            configurator.ConfigureEndpoints(context);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Porter API v1");
            });


            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
