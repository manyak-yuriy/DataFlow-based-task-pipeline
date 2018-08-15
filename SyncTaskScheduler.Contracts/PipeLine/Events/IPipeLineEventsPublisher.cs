namespace SyncTaskScheduler.Contracts.PipeLine.Events
{
    public interface IPipeLineEventsPublisher<TPipeLineElement>
    {
        void OnNewElementAvailable(IPipeLineProducer<TPipeLineElement> consumedBy, TPipeLineElement element);

        void OnPipeLineStopped();
    }
}
