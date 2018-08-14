using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncTaskScheduler.Contracts.Tasks;

namespace SyncTaskScheduler.Implementation.Tasks
{
    public class ParameterlessTask : ITask
    {
        private readonly Action _taskInternal;

        public ParameterlessTask(Action taskInternal)
        {
            _taskInternal = taskInternal;
        }

        public void Execute()
        {
            _taskInternal.Invoke();
        }
    }
}
