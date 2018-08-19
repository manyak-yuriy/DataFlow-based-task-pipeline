using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Implementation.PipeLine;

namespace SyncTaskScheduler.Implementation.Tests.ProducerConsumerPipeLine
{
    // Producers: one or more threads pushing items to a queue-like pipeline.
    // Consumer: a single thread which takes the items from the pipeline and processes them one-by-one.
    [TestFixture]
    public class ProducerConsumerPipeLineTests
    {
        private Mock<IPipeLineConsumer<Action>> _consumerMock;

        private static IEnumerable<object> CommonTestCaseSource
            => ProducerConsumerPipeLineTestCasesProvider.CommonTestCaseSource();

        [SetUp]
        public void OneTimeSetup()
        {
            _consumerMock = new Mock<IPipeLineConsumer<Action>>();

            _consumerMock
                .Setup(consumer => consumer.Consume(It.IsAny<Action>()))
                .Callback((Action a) =>
                {
                    a.Invoke();
                });
        }

        // Only one item at a time must be processed by the pipeline.
        [Test]
        [TestCaseSource(nameof(CommonTestCaseSource))]
        public void OneItemAtATimeIsProcessedByTheConsumer(int testActionDurationMs, int numberOfProducers, int maximumPipelineCapacity)
        {
            int sharedCounter = 0;
            int sharedCounterMismatchNumber = 0;

            var pipeLine = new ProducerConsumerPipeLine<Action>(_consumerMock.Object, maximumPipelineCapacity);

            // Increments the shared counter to 1 before performing a long-running task and decrements immediately after.
            Action testAction = () =>
            {
                // No interlocking is needed when comparing a shared value to a constant.
                if (!Equals(sharedCounter, 0))
                {
                    Interlocked.Increment(ref sharedCounterMismatchNumber);
                }

                Interlocked.Increment(ref sharedCounter);
                Thread.Sleep(testActionDurationMs);
                Interlocked.Decrement(ref sharedCounter);
            };

            List<Task> producerTasks = new List<Task>();

            for (int producerCounter = 0; producerCounter < numberOfProducers; producerCounter++)
            {
                var producer = Task.Run(async () => await pipeLine.EnqueueAsync(testAction));

                producerTasks.Add(producer);
            }

            Task.WaitAll(producerTasks.ToArray());

            pipeLine.CompleteProducing();
            pipeLine.PipeLineCompletion.Wait();

            // If the actions were invoked in a one-at-a-time fashion, there must be no mismathes.
            Assert.AreEqual(sharedCounterMismatchNumber, 0);
        }

        // Items must be processed by the consumer in the same order they were added by the producers.
        [Test]
        [TestCaseSource(nameof(CommonTestCaseSource))]
        public void ItemsAreProcessedInCorrectOrder(int testActionDurationMs, int numberOfProducers, int maximumPipelineCapacity)
        {
            var pipeLine = new ProducerConsumerPipeLine<Action>(_consumerMock.Object, maximumPipelineCapacity);

            object pushOrderLocker = new object();

            BlockingCollection<int> testActionsPushOrder = new BlockingCollection<int>();
            BlockingCollection<int> testActionsInvocationOrder = new BlockingCollection<int>();

            List<Task> producerTasks = new List<Task>();

            for (int producerCounter = 0; producerCounter < numberOfProducers; producerCounter++)
            {
                var testActionId = producerCounter;
                Action testAction = () =>
                {
                    testActionsInvocationOrder.Add(testActionId);
                    Thread.Sleep(testActionDurationMs);
                };

                var producer = Task.Run(() =>
                {
                    lock (pushOrderLocker)
                    {
                        pipeLine.EnqueueAsync(testAction).Wait();
                        testActionsPushOrder.Add(testActionId);
                    }
                });

                producerTasks.Add(producer);
            }

            Task.WaitAll(producerTasks.ToArray());
            pipeLine.CompleteProducing();
            pipeLine.PipeLineCompletion.Wait();

            _consumerMock.Verify(consumerMock => consumerMock.Consume(It.IsAny<Action>()), Times.Exactly(numberOfProducers));
            CollectionAssert.AreEqual(testActionsPushOrder, testActionsInvocationOrder);
        }

        // If the pipeline production completion is triggered and the completion is finished, every item should have been processed by this time
        // IMPORTANT: if capacity bounding is used, this will give the expected result only if the items are pushed asynchronously (which provided a support for postponing),
        // otherwise the sychronous method rejects the item immediately and it doesn't get a chance to be consumed in a postponed manner.
        [Test]
        [TestCaseSource(nameof(CommonTestCaseSource))]
        public void AllItemsShouldBeProcessedBeforePipeLineCompletion(int testActionDurationMs, int numberOfProducers, int maximumPipelineCapacity)
        {
            var pipeLine = new ProducerConsumerPipeLine<Action>(_consumerMock.Object, maximumPipelineCapacity);

            // Dummy test action.
            Action testAction = () =>
            {
                Thread.Sleep(testActionDurationMs);
            };

            List<Task> producerTasks = new List<Task>();

            for (int producerCounter = 0; producerCounter < numberOfProducers; producerCounter++)
            {
                var producer = Task.Run(async () => await pipeLine.EnqueueAsync(testAction));

                producerTasks.Add(producer);
            }

            Task.WaitAll(producerTasks.ToArray());
            pipeLine.CompleteProducing();
            pipeLine.PipeLineCompletion.Wait();

            _consumerMock.Verify(consumerMock => consumerMock.Consume(It.IsAny<Action>()), Times.Exactly(numberOfProducers));
        }
    }
}
