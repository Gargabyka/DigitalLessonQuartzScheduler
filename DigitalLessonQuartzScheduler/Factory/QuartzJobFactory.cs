using Quartz;
using Quartz.Spi;

namespace DigitalLessonQuartzScheduler.Factory
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QuartzJobFactory> _logger;

        public QuartzJobFactory(
            IServiceProvider serviceProvider, 
            ILogger<QuartzJobFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var jobType = bundle.JobDetail.JobType;

                _logger.LogInformation("Вызов Job:'{JobDetailKey}', class={JobTypeName}", bundle.JobDetail.Key, jobType.Name);
                
                var jobScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using var scope = jobScopeFactory.CreateScope();
                var job = (IJob?)scope.ServiceProvider.GetService(jobType);
                return job ?? throw new Exception("Не удалось найти Job");
            }
            catch (Exception e)
            {
                var message = $"Не удается вызвать job: '{bundle.JobDetail.Key}'";
                
                _logger.LogError(message, e);
                throw new Exception(message);
            }
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}