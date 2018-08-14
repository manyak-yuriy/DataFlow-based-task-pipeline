namespace SyncTaskScheduler.Contracts.Tasks
{
    public interface ITask<out TResult>
    {
        TResult Execute();
    }

    public interface ITask
    {
        void Execute();
    }
}
