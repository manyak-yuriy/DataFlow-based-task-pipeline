using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SyncTaskScheduler.Contracts.PipeLine;
using SyncTaskScheduler.Contracts.PipeLine.Events;
using SyncTaskScheduler.Contracts.Scheduler;
using SyncTaskScheduler.Contracts.Tasks;
using SyncTaskScheduler.Implementation.PipeLine;
using SyncTaskScheduler.Implementation.PipeLine.Events;
using SyncTaskScheduler.Implementation.Scheduler;
using SyncTaskScheduler.Implementation.Tasks;
using Console = System.Console;

namespace SyncTaskScheduler.ConsoleApp
{
    class Program
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        private static ITaskSchedulerService<ITask> CreateTaskSchedulerService()
        {
            // Normally, these would be injected via DI framework.
            var taskConsumer = new PausablePipeLineTaskConsumer();
            var pipeLine = new ProducerConsumerPipeLine<ITask>(taskConsumer, 3);

            var taskSchedulerService = new TaskSchedulerService<ITask>(pipeLine);

            return taskSchedulerService;
        }

        // This is just a really triviale use case.
        // See ProducerConsumerPipeLineTests to validate all requirements.
        static void Main(string[] args)
        {
            var taskSchedulerService = CreateTaskSchedulerService();
            bool stopped = false;

            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;

                Console.WriteLine("Stopping the scheduling service. Waiting for the queue exhaustion...");
                taskSchedulerService.StopAsync().Wait();

                stopped = true;
                Console.WriteLine("Stopped the scheduling service.");
            };

            do
            {
                GenerateTasksInParallel(taskSchedulerService);

                Console.WriteLine("Press any key to add new tasks. Press Ctrl+C to wait for the queue and exit.");
                Console.ReadKey();
            } while (!stopped);
        }

        private static void GenerateTasksInParallel(ITaskSchedulerService<ITask> taskSchedulerService)
        {
            for (int sleepTime = 5; sleepTime >= 0; sleepTime--)
            {
                var time = sleepTime;

                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Adding a task. Duration: {0}00 ms", time);

                    taskSchedulerService.PushToStartAsync(new ActionBasedTask<int>((int timeToSleepInSec) =>
                    {
                        Console.WriteLine("Start sleeping for {0}00 ms", timeToSleepInSec);
                        Thread.Sleep(100 * timeToSleepInSec);
                        Console.WriteLine("Woke up after {0}00 ms", timeToSleepInSec);
                    }, time));
                });
            }
        }
    }
}
