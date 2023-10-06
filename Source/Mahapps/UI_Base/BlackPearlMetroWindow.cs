using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using BlackPearl.PrismUI;

using MahApps.Metro.Controls;

namespace BlackPearl.Mahapps
{
    public abstract class BlackPearlMetroWindow : MetroWindow
    {
        public BlackPearlMetroWindow()
        {
            Loaded += Window_Loaded;
            Unloaded += Window_Unloaded;

            (this as IComponentConnector)?.InitializeComponent();
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= Window_Unloaded;

            try
            {
                await this.DataContextAction<BlackPearlViewModel>(vm => vm.OnUnload());
                await this.DataContextAction<IDisposable>(vm =>
                {
                    vm.Dispose();
                    return Task.CompletedTask;
                });
            }
            catch { }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Window_Loaded;

            try
            {
                await this.DataContextAction<BlackPearlViewModel>(vm => vm.OnLoad());
            }
            catch { }
        }
    }
}