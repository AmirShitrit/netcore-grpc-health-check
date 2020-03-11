using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpApiWithHealthChecks
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
            services.AddControllers();

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
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(Liveness)
                });

                endpoints.MapHealthChecks("/readiness", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(Readiness)
                });

                endpoints.MapControllers();
            });
        }
    }
}
