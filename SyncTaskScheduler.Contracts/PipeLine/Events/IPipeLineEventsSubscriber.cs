using System;
using SyncTaskScheduler.Contracts.EventHandling;

namespace SyncTaskScheduler.Contracts.PipeLine.Events
{
    public interface IPipeLineEventsSubscriber<TPipeLineElement>
    {
        event EventHandler<NewElementAvailableEventArgs<TPipeLineElement>> NewElementAvailable;
        event Action ProductionStopped;
    }
}
