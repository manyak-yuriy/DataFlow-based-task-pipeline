using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Implementation.PipeLine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SyncTaskScheduler.Tests
{
    // Producers: one or more threads pushing items to a queue-like pipeline.
    // Consumer: a single thread which takes the items from the pipeline and processes them one-by-one.
    [TestFixture]
    public class ProducerConsumerPipeLineTests
    {
        // Only one item at a time must be processed by the pipeline.
        [Test]
        public void OneItemAtATimeIsProcessedByTheConsumerTest()
        {
            var consumerMock = new Mock<IPipeLineConsumer<Action>>();

            consumerMock
                .Setup(consumer => consumer.Consume(It.IsAny<Action>()))
                .Callback((Action a) =>
                {
                    a.Invoke();
                });

            var pipeLine = new ProducerConsumerPipeLine<Action>(consumerMock.Object, 3);

            int sharedCounter = 0;
            int sharedCounterMismatchNumber = 0;

            // Increments the shared counter to 1 while performing a long-running task and decrements immediately after.
            Action testAction = () =>
            {
                // No interlocking is needed when comparing a shared value to a constant.
                if (!Equals(sharedCounter, 0))
                {
                    Interlocked.Increment(ref sharedCounterMismatchNumber);
                }

                Interlocked.Increment(ref sharedCounter);
                Thread.Sleep(2000);
                Interlocked.Decrement(ref sharedCounter);
            };

            List<Task> producerTasks = new List<Task>();

            for (int producerCounter = 0; producerCounter < 5; producerCounter++)
            {
                var producer = Task.Run(async () => await pipeLine.EnqueueAsync(testAction));

                producerTasks.Add(producer);
            }

            // Will throw if any producer thread throwed.
            Task.WaitAll(producerTasks.ToArray());

            // If the actions were invoked on a one-at-a-time basis, sharedCounterMismatchNumber must be 0 again.
            Assert.AreEqual(sharedCounterMismatchNumber, 0);
        }

        // Items must be processed by the consumer in the same order they were added by the producers.
        [Test]
        public void ItemsAreProcessedInTheCorrectOrder()
        {

        }

        // The pipeline must block the producer threads if the maximum capacity is reached due to a slow consumer. 
        [Test]
        public void ThrottlingIsAppliedForSlowTasks()
        {

        }
    }
}
