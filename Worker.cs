using Grpc_Worker_Service.GrpcServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Grpc_Worker_Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder
                    .ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(0, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    })
                    .UseKestrel()
                    .UseStartup<GrpcServerStartup>();
            })
            .Build()
            .StartAsync(stoppingToken);
    }
}

public class GrpcServerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddSingleton<GreeterService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<GreeterService>();
        });
    }
}
