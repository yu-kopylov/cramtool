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
            TEventSource newSource = (TEventSource) changedArgs.NewValue;
            UpdateListener(oldSource, newSource, eventName, handler);
        }

        public static void UpdateListener<TEventSource, TEventArgs>(TEventSource oldSource, TEventSource newSource, string eventName, EventHandler<TEventArgs> handler)
            where TEventSource : class
            where TEventArgs : EventArgs
        {
            if (oldSource != null)
            {
                WeakEventManager<TEventSource, TEventArgs>.RemoveHandler(oldSource, eventName, handler);
            }

            if (newSource != null)
            {
                WeakEventManager<TEventSource, TEventArgs>.AddHandler(newSource, eventName, handler);
            }
        }
    }
}