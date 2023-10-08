using Quartz;

namespace DigitalLessonQuartzScheduler.Factory
{
    public interface IQuartzFactory
    {
        public IScheduler GetScheduler();
    }
}