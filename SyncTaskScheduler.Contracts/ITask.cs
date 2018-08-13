namespace SyncTaskScheduler.Contracts
{
    public interface ITask<in TArgs, out TResult>
    {
        TResult Execute(TArgs args);
    }
}
