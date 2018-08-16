using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.Scheduler;

namespace SyncTaskScheduler.Implementation.Scheduler
{
    public class TaskScheduler<TPipeLineItem> : ITaskScheduler<TPipeLineItem>
    {
        private readonly IPipeLineProducer<TPipeLineItem> _pipeLineProducer;
        private readonly IPipeLineConsumer<TPipeLineItem> _pipeLineConsumer;

        public TaskScheduler(IPipeLineProducer<TPipeLineItem> pipeLineProducer, IPipeLineConsumer<TPipeLineItem> pipeLineConsumer)
        {
            _pipeLineProducer = pipeLineProducer;
            _pipeLineConsumer = pipeLineConsumer;
        }


    }
}
