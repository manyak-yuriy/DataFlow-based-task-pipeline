using System;
using System.Threading.Tasks.Dataflow;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.PipeLine.Events;
using System.Threading.Tasks;

namespace SyncTaskScheduler.Implementation.PipeLine
{
    public class ProducerConsumerPipeLine<TPipeLineElement> : IPipeLineConsumer, IPipeLineProducer<TPipeLineElement>
    {
        private readonly IPipeLineEventsPublisher<TPipeLineElement> _pipeLineEventsPublisher;
        private readonly BufferBlock<TPipeLineElement> _buffer;

        public ProducerConsumerPipeLine(IPipeLineEventsPublisher<TPipeLineElement> pipeLineEventsPublisher, int maximumCapacity)
        {
            _pipeLineEventsPublisher = pipeLineEventsPublisher;

            _buffer = new BufferBlock<TPipeLineElement>(new DataflowBlockOptions()
            {
                BoundedCapacity = maximumCapacity
            });
        }

        public async Task StartConsumeElementsAsync()
        {
            while (await _buffer.OutputAvailableAsync())
            {
                var element = _buffer.Receive();

                _pipeLineEventsPublisher.OnNewElementAvailable(this, element);
            }

            _pipeLineEventsPublisher.OnPipeLineStopped();
        }

        public void StopPipeLine()
        {
            _buffer.Complete();
        }

        public void EnQueue(TPipeLineElement element)
        {
            _buffer.Post(element);
        }
    }
}
