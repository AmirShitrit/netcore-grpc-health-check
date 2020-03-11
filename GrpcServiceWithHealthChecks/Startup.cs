using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServiceWithHealthChecks
{
    public class Startup
    {
        private const string Liveness = "Liveness";
        private const string Readiness = "Readiness";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            string dbConnectionString = Configuration.GetConnectionString("OperationalDB");
            string redisConnectionString = Configuration.GetConnectionString("Cache");

            services.AddHealthChecks()
                .AddSqlServer(dbConnectionString, tags: new[] { Liveness, Readiness })
                .AddRedis(redisConnectionString, tags: new[] { Readiness });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcHealthCheckService>();

                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
