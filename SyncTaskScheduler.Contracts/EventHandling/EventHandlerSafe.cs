using System;
using NLog;

namespace SyncTaskScheduler.Contracts.EventHandling
{
    public class EventHandlerSafe<T> where T : EventArgs
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();

        private Delegate _eventHandler;

        public event EventHandler<T> Handler
        {
            add
            {
                lock (_lock)
                {
                    _eventHandler = Delegate.Combine(_eventHandler, value);
                }
            }

            remove
            {
                lock (_lock)
                {
                    _eventHandler = Delegate.Remove(_eventHandler, value);
                }
            }
        }

        public void RaiseEvent(object sender, T arg)
        {
            lock (_lock)
            {
                if (_eventHandler != null)
                {
                    foreach (var handler in _eventHandler.GetInvocationList())
                    {
                        try
                        {
                            handler.DynamicInvoke(sender, arg);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, "The event handler failed to execute.");
                        }
                    }
                }
            }
        }
    }
}
