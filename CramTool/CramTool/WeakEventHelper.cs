using System;
using System.Windows;

namespace CramTool
{
    public static class WeakEventHelper
    {
        public static void UpdateListener<TEventSource, TEventArgs>(DependencyPropertyChangedEventArgs changedArgs, string eventName, EventHandler<TEventArgs> handler)
            where TEventSource : class
            where TEventArgs : EventArgs
        {
            TEventSource oldSource = (TEventSource) changedArgs.OldValue;
            if (oldSource != null)
            {
                WeakEventManager<TEventSource, TEventArgs>.RemoveHandler(oldSource, eventName, handler);
            }

            TEventSource newSource = (TEventSource) changedArgs.NewValue;
            if (newSource != null)
            {
                WeakEventManager<TEventSource, TEventArgs>.AddHandler(newSource, eventName, handler);
            }
        }
    }
}