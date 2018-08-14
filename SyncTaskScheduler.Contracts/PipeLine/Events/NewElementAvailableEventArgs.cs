using System;

namespace SyncTaskScheduler.Contracts.PipeLine.Events
{
    public class NewElementAvailableEventArgs<TPipeLineElement> : EventArgs
    {
        public NewElementAvailableEventArgs(TPipeLineElement element)
        {
            Element = element;
        }

        public TPipeLineElement Element
        {
            get; private set;
        }
    }
}
