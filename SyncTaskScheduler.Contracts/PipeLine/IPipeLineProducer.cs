namespace SyncTaskScheduler.Contracts.PipeLine
{
    public interface IPipeLineProducer<in TPipeLineElement> : IPipeLine
    {
        void EnQueue(TPipeLineElement element);
    }
}
