using System;
using SyncTaskScheduler.Contracts.Tasks;

namespace SyncTaskScheduler.Implementation.Tasks
{
    public class FuncBasedTask<TArgs, TResult> : ITask<TResult>
    {
        private readonly Func<TArgs, TResult> _taskInternal;
        private readonly TArgs _args;

        public FuncBasedTask(Func<TArgs, TResult> taskInternal, TArgs args)
        {
            _taskInternal = taskInternal;
            _args = args;
        }

        public TResult Execute()
        {
            return Execute(_args);
        }

        private TResult Execute(TArgs args)
        {
            return _taskInternal.Invoke(args);
        }
    }
}
