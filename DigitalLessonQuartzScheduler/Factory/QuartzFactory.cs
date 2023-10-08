using Quartz;
using Quartz.Impl;

namespace DigitalLessonQuartzScheduler.Factory
{
    public class QuartzFactory : IQuartzFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QuartzFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IScheduler GetScheduler()
        {
            var logger = _serviceProvider.GetService<ILogger<QuartzJobFactory>>();

            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.JobFactory = new QuartzJobFactory(_serviceProvider, logger);
            return scheduler;
        }
    }
}