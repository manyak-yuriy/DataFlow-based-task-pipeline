using System.Threading.Tasks;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.Tasks;
using SyncTaskScheduler.Implementation.Tasks;

namespace SyncTaskScheduler.Implementation.PipeLine
{
    public class PipeLineTaskConsumer : IPipeLineConsumer<ITask>
    {
        public void Consume(ITask element)
        {
            element.Execute();
        }
    }
}
