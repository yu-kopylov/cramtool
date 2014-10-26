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
            TEventSource oldWord = (TEventSource) changedArgs.OldValue;
            if (oldWord != null)
            {
                WeakEventManager<TEventSource, TEventArgs>.RemoveHandler(oldWord, "PropertyChanged", handler);
            }

            TEventSource newWord = (TEventSource) changedArgs.NewValue;
            if (newWord != null)
            {
                WeakEventManager<TEventSource, TEventArgs>.AddHandler(newWord, "PropertyChanged", handler);
            }
        }
    }
}