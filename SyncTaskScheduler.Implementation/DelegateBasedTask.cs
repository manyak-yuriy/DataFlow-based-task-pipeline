using System;
using SyncTaskScheduler.Contracts;

namespace SyncTaskScheduler.Implementation
{
    public class DelegateBasedTask<TArgs, TResult> : ITask<TArgs, TResult>
    {
        private readonly Func<TArgs, TResult> _taskInternal;

        public DelegateBasedTask(Func<TArgs, TResult> taskInternal, bool executeOnOriginalThread = false)
        {
            _taskInternal = taskInternal;
        }

        public TResult Execute(TArgs args)
        {
            return _taskInternal.Invoke(args);
        }
    }
}
