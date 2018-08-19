using System.Threading.Tasks;

namespace SyncTaskScheduler.Contracts.PipeLine
{
    public interface IPipeLineProducer<in TPipeLineItem> : IPipeLine<TPipeLineItem>
    {
        Task<bool> EnqueueAsync(TPipeLineItem item);

        bool Enqueue(TPipeLineItem item);
    }
}
