using System.Threading.Tasks;

namespace SyncTaskScheduler.Contracts.PipeLine
{
    public interface IPipeLineConsumer : IPipeLine
    {
        Task StartConsumeElementsAsync();
    }
}