using DigitalLessonQuartzScheduler.Factory;
using DigitalLessonQuartzScheduler.Jobs;
using Quartz;
using Quartz.Spi;

namespace DigitalLessonQuartzScheduler
{
    public static class QuartzRegistration
    {
        public static void QuartzRegister(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            RegisterFactory(serviceCollection);
            RegisterJob(serviceCollection);

            serviceCollection.AddQuartz(option =>
            {
                option.SchedulerId = "QuartzScheduler.DigitalLesson";
                option.SchedulerName = "Quartz.AspNetCore.Scheduler";
                
                option.UseJobFactory<QuartzJobFactory>();

                option.MaxBatchSize = 5;
                option.InterruptJobsOnShutdown = true;
                option.InterruptJobsOnShutdownWithWait = true;

                CleanJob(option, configuration);
            });

            serviceCollection.AddQuartzHostedService(option =>
            {
                option.StartDelay = TimeSpan.FromMilliseconds(1_000);
                option.AwaitApplicationStarted = true;
                option.WaitForJobsToComplete = true;
            });
        }

        private static void RegisterFactory(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IJobFactory, QuartzJobFactory>();
            serviceCollection.AddSingleton<IQuartzFactory, QuartzFactory>();
        }

        private static void RegisterJob(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<CleanJob>();
        }

        private static void CleanJob(IServiceCollectionQuartzConfigurator quartzConfigurator, IConfiguration configuration)
        {
            const string jobKey = nameof(ApiDigitalLesson.gRPC.CleanJob);
            var cron = configuration["CleanJob:CronSettings"];

            if (string.IsNullOrEmpty(cron))
            {
                throw new Exception("Не задано cron выражение для CleanJob");
            }

            quartzConfigurator.AddJob<CleanJob>(o => o.WithIdentity(jobKey));
            quartzConfigurator.AddTrigger(trigger =>
            {
                trigger.WithIdentity($"{jobKey}-trigger");
                trigger.ForJob(jobKey);
                trigger.StartNow();
                trigger.WithCronSchedule(cron);
            });
        }
    }
}