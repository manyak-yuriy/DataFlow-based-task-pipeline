using System;
using SyncTaskScheduler.Contracts;
using SyncTaskScheduler.Contracts.EventHandling;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.PipeLine.Events;

namespace SyncTaskScheduler.Implementation.PipeLine.Events
{
    public class PipeLineEventsProvider<TPipeLineItem> : IPipeLineEventsSubscriber<TPipeLineItem>, IPipeLineEventsPublisher<TPipeLineItem>
    {
        public event EventHandler<NewElementAvailableEventArgs<TPipeLineItem>> NewElementAvailable;
        public event Action ProductionStopped;

        public void OnNewElementAvailable(IPipeLineProducer<TPipeLineItem> consumedBy, TPipeLineItem element)
        {
            NewElementAvailable?.Invoke(consumedBy, new NewElementAvailableEventArgs<TPipeLineItem>(element));
        }

        public void OnPipeLineStopped()
        {
            ProductionStopped?.Invoke();
        }
    }
}
