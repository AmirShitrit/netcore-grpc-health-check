using Grpc.Health.V1;
using Grpc.Net.Client;
using System;

namespace gRPC_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the check.");
            Console.ReadKey(true);


            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Health.HealthClient(channel);
            var req = new HealthCheckRequest
            {
                Service = "Liveness"
            };

            try
            {
                var res = client.Check(req);
                Console.WriteLine(res.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey(true);
        }
    }
}
