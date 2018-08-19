using System.Threading.Tasks;

namespace SyncTaskScheduler.Contracts.Scheduler
{
    public interface ITaskSchedulerService<in TTask>
    {
        Task PushToStartAsync(TTask task);

        Task StopAsync();
    }
}
