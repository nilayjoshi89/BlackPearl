using System.Runtime.Versioning;
using System.Windows;

using BlackPearl.Mahapps;
using BlackPearl.PrismUI;

using MahApps.Metro.Controls;

using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;

namespace BlackPearl.Controls.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var result = Container.Resolve<ShellWindow>();
            return result;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDispatcherService, DispatcherService>()
                            .Register<ShellWindowViewModel>()
                            .Register<ShellWindow>()
                            .Register<MultiSelectComboBoxDemoViewModel>()
                            .Register<MultiSelectComboBoxDemoView>()
                            .RegisterForNavigation<ShellWindow, ShellWindowViewModel>();

            containerRegistry.RegisterForNavigation<MultiSelectComboBoxDemoView, MultiSelectComboBoxDemoViewModel>(Constants.MultiSelectComboBoxView);
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping<HamburgerMenu, HamburgerMenuSingleRegionAdapter>();
        }
    }
}
