using System;
using SyncTaskScheduler.Contracts;
using SyncTaskScheduler.Contracts.EventHandling;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.PipeLine.Events;

namespace SyncTaskScheduler.Implementation.PipeLine.Events
{
    public class PipeLineEventsProvider<TPipeLineElement> : IPipeLineEventsSubscriber<TPipeLineElement>, IPipeLineEventsPublisher<TPipeLineElement>
    {
        public event EventHandler<NewElementAvailableEventArgs<TPipeLineElement>> NewElementAvailable;
        public event Action ProductionStopped;

        public void OnNewElementAvailable(IPipeLineProducer<TPipeLineElement> consumedBy, TPipeLineElement element)
        {
            NewElementAvailable?.Invoke(consumedBy, new NewElementAvailableEventArgs<TPipeLineElement>(element));
        }

        public void OnPipeLineStopped()
        {
            ProductionStopped?.Invoke();
        }
    }
}
