using System;
using System.Threading.Tasks;

namespace SyncTaskScheduler.Contracts.PipeLine
{
    public interface IPipeLineConsumer<in TPipeLineItem>
    {
        void Consume(TPipeLineItem element);
    }
}