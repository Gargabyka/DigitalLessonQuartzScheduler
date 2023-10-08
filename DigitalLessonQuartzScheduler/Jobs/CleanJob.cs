using ApiDigitalLesson.gRPC;
using Grpc.Health.V1;
using Quartz;

namespace DigitalLessonQuartzScheduler.Jobs
{
    public class CleanJob : IJob
    {
        private readonly ApiDigitalLesson.gRPC.CleanJob.CleanJobClient _client;
        private readonly Health.HealthClient _healthClient;
        private readonly ILogger<CleanJob> _logger;
        private readonly IConfiguration _configuration;

        public CleanJob(
            ApiDigitalLesson.gRPC.CleanJob.CleanJobClient client, 
            ILogger<CleanJob> logger, 
            IConfiguration configuration, Health.HealthClient healthClient)
        {
            _client = client;
            _logger = logger;
            _configuration = configuration;
            _healthClient = healthClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var check = await _healthClient.CheckAsync(new HealthCheckRequest());

                if (check.Status != HealthCheckResponse.Types.ServingStatus.Serving)
                {
                    throw new Exception("Не удалось связаться с gRPC сервером");
                }
                
                var mount = _configuration["CleanJob:CleanDate"];

                var request = new CleanupRequest
                {
                    Mount = Convert.ToInt32(mount)
                };
                
                await _client.CleanJobAsyncAsync(request);
            }
            catch (Exception e)
            {
                var message = $"Не удалось отправить запрос на удаление данных: {e.Message}";
                _logger.LogError(message);
                throw new Exception(message);
            }
        }
    }
}