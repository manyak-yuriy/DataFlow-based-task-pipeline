using System.Threading;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.Tasks;

namespace SyncTaskScheduler.Implementation.PipeLine
{
    public class PausablePipeLineTaskConsumer : PipeLineTaskConsumer, IPausablePipeLineConsumer<ITask>
    {
        private readonly EventWaitHandle _waitHandle;

        public PausablePipeLineTaskConsumer()
        {
            _waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        public PausablePipeLineTaskConsumer(EventWaitHandle waitHandle)
        {
            _waitHandle = waitHandle;
        }

        public bool Pause()
        {
            _waitHandle.Reset();

            return true;
        }

        public bool Resume()
        {
            _waitHandle.Set();

            return true;
        }

        public override void Consume(ITask item)
        {
            _waitHandle.WaitOne();

            base.Consume(item);
        }
    }
}
