using System.Threading.Tasks;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.Tasks;
using SyncTaskScheduler.Implementation.Tasks;

namespace SyncTaskScheduler.Implementation.PipeLine
{
    public class PipeLineTaskConsumer : IPipeLineConsumer<ITask>
    {
        public virtual void Consume(ITask item)
        {
            item.Execute();
        }
    }
}
