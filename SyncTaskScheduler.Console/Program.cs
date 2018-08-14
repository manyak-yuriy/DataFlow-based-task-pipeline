using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.PipeLine.Events;
using SyncTaskScheduler.Implementation.PipeLine;
using SyncTaskScheduler.Implementation.PipeLine.Events;
using SyncTaskScheduler.Implementation.Tasks;
using Console = System.Console;

namespace SyncTaskScheduler.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeLineEventsProvider = new PipeLineEventsProvider<ActionBasedTask<int>>();
            var pipeLine = new ProducerConsumerPipeLine<ActionBasedTask<int>>(pipeLineEventsProvider);

            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;
                pipeLine.StopPipeLine();
            };
            
            var executeTasks = ExecuteTasksOneByOne(pipeLine, pipeLineEventsProvider);
            GenerateTasksInParallel(pipeLine);

            executeTasks.Wait();
        }

        private static void GenerateTasksInParallel(IPipeLineProducer<ActionBasedTask<int>> pipeLineProducer)
        {
            List<Task> tasks = new List<Task>();

            for (int sleepTime = 3; sleepTime >= 0; sleepTime--)
            {
                var time = sleepTime;

                var task = Task.Factory.StartNew(() =>
                {
                    pipeLineProducer.EnQueue(new ActionBasedTask<int>((int timeToSleepInSec) =>
                    {
                        Console.WriteLine("Start sleeping for {0} seconds", timeToSleepInSec);
                        Thread.Sleep(1000 * timeToSleepInSec);
                        Console.WriteLine("Woke up after {0} seconds", timeToSleepInSec);
                    }, time));
                });

                tasks.Add(task);
            }

            //Task.WaitAll(tasks.ToArray());

            //pipeLineProducer.StopPipeLine();
        }

        private static Task ExecuteTasksOneByOne(IPipeLineConsumer pipeLineConsumer, IPipeLineEventsSubscriber<ActionBasedTask<int>> pipeLineEventsProvider)
        {
            return Task.Factory.StartNew(() =>
            {
                pipeLineEventsProvider.NewElementAvailable += (sender, e) => e.Element.Execute();

                Console.WriteLine("Started consuming tasks");

                pipeLineConsumer.StartConsumeElementsFromPipeLineAsync().Wait();

                Console.WriteLine("Finished consuming tasks");
            });
        }
    }
}
