using ApiDigitalLesson.gRPC;
using Grpc.Health.V1;

namespace DigitalLessonQuartzScheduler
{
    public static class GrpcRegistration
    {
        public static void AddGrpcClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connection = configuration["GrpcServer:Connection"];

            if (string.IsNullOrEmpty(connection))
            {
                throw new Exception("Не передан url до gRPC");
            }
            
            var address = new Uri(connection);
            

            serviceCollection.AddGrpcClient<Health.HealthClient>(o => o.Address = address);
            
            serviceCollection.AddGrpcClient<CleanJob.CleanJobClient>(o => o.Address = address);
            serviceCollection.AddGrpcClient<NotificationJob.NotificationJobClient>(o => o.Address = address);
        }
    }
}