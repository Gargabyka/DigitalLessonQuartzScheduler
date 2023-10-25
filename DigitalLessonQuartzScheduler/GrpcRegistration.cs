using ApiDigitalLesson.gRPC;
using Grpc.Health.V1;

namespace DigitalLessonQuartzScheduler
{
    public static class GrpcRegistration
    {
        private static Uri Address { get; set; } = null!;
        private static IServiceCollection ServiceCollection { get; set; } = null!;

        public static void AddGrpcClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            Configuration(serviceCollection, configuration);
            
            RegisterGrpc<Health.HealthClient>();
            
            RegisterGrpc<CleanJob.CleanJobClient>();
            RegisterGrpc<NotificationJob.NotificationJobClient>();
        }

        private static void Configuration(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            ServiceCollection = serviceCollection;
            
            var connection = configuration["GrpcServer:Connection"];

            if (string.IsNullOrEmpty(connection))
            {
                throw new Exception("Не передан url до gRPC");
            }
            
            Address = new Uri(connection);
        }

        private static void RegisterGrpc<TClient>() where TClient : class
        {
            ServiceCollection.AddGrpcClient<TClient>(o => o.Address = Address);
        }
    }
}