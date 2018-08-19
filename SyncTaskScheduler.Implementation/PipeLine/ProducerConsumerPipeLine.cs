using System.Threading.Tasks.Dataflow;
using SyncTaskScheduler.Contracts.PipeLine;
using System.Threading.Tasks;

namespace SyncTaskScheduler.Implementation.PipeLine
{
    public class ProducerConsumerPipeLine<TPipeLineItem> : IPipeLineProducer<TPipeLineItem>
    {
        private readonly IPipeLineConsumer<TPipeLineItem> _pipeLineConsumer;

        private readonly BufferBlock<TPipeLineItem> _bufferBlock;
        private readonly ActionBlock<TPipeLineItem> _actionBlock;

        public ProducerConsumerPipeLine(IPipeLineConsumer<TPipeLineItem> pipeLineConsumer, int maximumCapacity)
        {
            _pipeLineConsumer = pipeLineConsumer;

            _bufferBlock = new BufferBlock<TPipeLineItem>(new DataflowBlockOptions
            {
                BoundedCapacity = maximumCapacity
            });

            _actionBlock = new ActionBlock<TPipeLineItem>(
                (element) => _pipeLineConsumer.Consume(element),
                new ExecutionDataflowBlockOptions()
                {
                    BoundedCapacity = 1,
                    // This is set explicitly to 1 in order to make the consumer process items one at a time.
                    MaxDegreeOfParallelism = 1
                });

            var linkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };

            _bufferBlock.LinkTo(_actionBlock, linkOptions);
        }

        public IPipeLineConsumer<TPipeLineItem> Consumer => _pipeLineConsumer;

        public void CompleteProducing()
        {
            _bufferBlock.Complete();
        }

        public Task PipeLineCompletion => _actionBlock.Completion;

        public async Task<bool> EnqueueAsync(TPipeLineItem item)
        {
            return await _bufferBlock.SendAsync(item);
        }

        public bool Enqueue(TPipeLineItem item)
        {
            return _bufferBlock.Post(item);
        }
    }
}
