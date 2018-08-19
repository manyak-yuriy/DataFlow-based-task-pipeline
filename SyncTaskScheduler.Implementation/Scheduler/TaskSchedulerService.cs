using System.Threading.Tasks;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.Scheduler;
using SyncTaskScheduler.Contracts.Tasks;

namespace SyncTaskScheduler.Implementation.Scheduler
{
    public class TaskSchedulerService<TTask> : ITaskSchedulerService<TTask>, IPausable
        where TTask : ITask
    {
        private readonly IPipeLineProducer<TTask> _taskPipeLineProducer;

        public TaskSchedulerService(IPipeLineProducer<TTask> taskPipeLineProducer)
        {
            _taskPipeLineProducer = taskPipeLineProducer;
        }

        public async Task PushToStartAsync(TTask task)
        {
            await _taskPipeLineProducer.EnqueueAsync(task);
        }

        public bool Pause()
        {
            var pausableConsumer = _taskPipeLineProducer.Consumer as IPausable;

            if (pausableConsumer != null)
            {
                return pausableConsumer.Pause();
            }

            return false;
        }

        public bool Resume()
        {
            var pausableConsumer = _taskPipeLineProducer.Consumer as IPausable;

            if (pausableConsumer != null)
            {
                return pausableConsumer.Resume();
            }

            return false;
        }

        public async Task StopAsync()
        {
            _taskPipeLineProducer.CompleteProducing();

            await _taskPipeLineProducer.PipeLineCompletion;
        }
    }
}
