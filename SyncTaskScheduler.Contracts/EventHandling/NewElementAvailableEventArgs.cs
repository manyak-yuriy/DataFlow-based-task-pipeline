using System;

namespace SyncTaskScheduler.Contracts.EventHandling
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
