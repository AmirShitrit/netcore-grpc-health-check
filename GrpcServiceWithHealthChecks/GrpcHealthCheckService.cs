using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading.Tasks;
using static Grpc.Health.V1.HealthCheckResponse.Types;

namespace GrpcServiceWithHealthChecks
{
    public class GrpcHealthCheckService : Health.HealthBase
    {
        private readonly HealthCheckService _healthCheckService;

        public GrpcHealthCheckService(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            Func<HealthCheckRegistration, bool> GetHealthCheckPredicate()
            {
                string[] tags = request.Service?.Split(";") ?? Array.Empty<string>();

                static bool PassAlways(HealthCheckRegistration _) => true;

                if (tags.Length == 0)
                {
                    return PassAlways;
                }

                bool CheckContainsTags(HealthCheckRegistration healthCheck) =>
                    healthCheck.Tags.IsSupersetOf(tags);

                return CheckContainsTags;
            }

            var predicate = GetHealthCheckPredicate();

            var result = await _healthCheckService.CheckHealthAsync(predicate, context.CancellationToken);

            var status = result.Status == HealthStatus.Healthy ? ServingStatus.Serving : ServingStatus.NotServing;

            return new HealthCheckResponse
            {
                Status = status
            };
        }
    }
}
