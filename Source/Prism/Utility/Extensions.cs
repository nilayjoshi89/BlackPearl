using System;
using System.Threading.Tasks;
using System.Windows;

using Prism.Events;

namespace BlackPearl.PrismUI
{
    public static class Extensions
    {
        public static async Task<bool> DataContextAction<T>(this FrameworkElement frameworkElement, Func<T, Task> action)
        {
            if (!(frameworkElement?.DataContext is T vm) || vm == null)
            {
                return false;
            }

            await action(vm);
            return true;
        }
        public static IEventAggregator Subscribe<TPayload>(this IEventAggregator eventAggregator, Action<TPayload> action)
        {
            eventAggregator.GetEvent<PubSubEvent<TPayload>>()
                .Subscribe(action, ThreadOption.BackgroundThread, keepSubscriberReferenceAlive: false);
            return eventAggregator;
        }
        public static IEventAggregator Publish<TPayload>(this IEventAggregator eventAggregator, TPayload data)
        {
            eventAggregator.GetEvent<PubSubEvent<TPayload>>()
                .Publish(data);
            return eventAggregator;
        }
    }
}