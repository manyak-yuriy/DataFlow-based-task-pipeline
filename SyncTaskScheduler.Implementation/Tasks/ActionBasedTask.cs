using SyncTaskScheduler.Contracts.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTaskScheduler.Implementation.Tasks
{
    public class ActionBasedTask<TArgs> : ITask
    {
        private readonly Action<TArgs> _taskInternal;
        private readonly TArgs _args;

        public ActionBasedTask(Action<TArgs> taskInternal, TArgs args)
        {
            _taskInternal = taskInternal;
            _args = args;
        }

        public void Execute()
        {
            Execute(_args);
        }

        private void Execute(TArgs args)
        {
            _taskInternal.Invoke(args);
        }
    }
}
