using System.Threading.Tasks;

namespace SyncTaskScheduler.Contracts.PipeLine
{
    public interface IPipeLineProducer<in TPipeLineItem> : IPipeLine
    {
        Task EnqueueAsync(TPipeLineItem item);
    }
}
