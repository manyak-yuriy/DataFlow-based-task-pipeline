using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTaskScheduler.Contracts.PipeLine
{
    public interface IPipeLine<in TPipeLineItem>
    {
        void CompleteProducing();

        Task PipeLineCompletion { get; }

        IPipeLineConsumer<TPipeLineItem> Consumer { get; }
    }
}
