using System;
using System.Threading.Tasks;
using System.Windows;

namespace BlackPearl.PrismUI
{
    public interface IDispatcherService
    {
        Task<bool> Execute(Action action);
    }

    public class DispatcherService : IDispatcherService
    {
        public async Task<bool> Execute(Action action)
        {
            bool isSuccess = true;
            if (Application.Current.Dispatcher.CheckAccess())
            {
                try
                {
                    action();
                }
                catch
                {
                    isSuccess = false;
                }
                return isSuccess;
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    action();
                }
                catch
                {
                    isSuccess = false;
                }
            });

            return isSuccess;
        }
    }
}
