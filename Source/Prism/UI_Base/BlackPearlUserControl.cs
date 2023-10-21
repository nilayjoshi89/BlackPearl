using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

using Prism.Regions;

namespace BlackPearl.PrismUI
{
    public abstract class BlackPearlUserControl : UserControl
    {
        public BlackPearlUserControl()
        {
            Loaded += Control_Loaded;
            Unloaded += Control_Unloaded;

            (this as IComponentConnector)?.InitializeComponent();
        }

        private async void Control_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Unloaded -= Control_Unloaded;

            try
            {
                if (DataContext is IRegionMemberLifetime rlm && rlm.KeepAlive)
                {
                    return;
                }

                await this.DataContextAction<BlackPearlViewModel>(vm => vm.OnUnload());
                await this.DataContextAction<IDisposable>(vm =>
                {
                    vm.Dispose();
                    return Task.CompletedTask;
                });
            }
            catch { }
        }

        private async void Control_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= Control_Loaded;

            try
            {
                await this.DataContextAction<BlackPearlViewModel>(vm => vm.OnLoad());
            }
            catch { }
        }
    }
}
