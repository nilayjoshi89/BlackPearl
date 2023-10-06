using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using BlackPearl.PrismUI;

using MahApps.Metro.Controls;

using Prism.Commands;
using Prism.Regions;

namespace BlackPearl.Controls.Demo
{

#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public class ShellWindowViewModel : BlackPearlViewModel
    {
        private readonly IRegionManager regionManager;

        public ShellWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public ObservableCollection<HamburgerMenuIconItem> MenuSource { get; set; } = Constants.DefaultMenuItems;

        public override Task OnLoad()
        {
            foreach (var item in MenuSource)
            {
                item.Command = new DelegateCommand<object>(n => regionManager.RequestNavigate(Constants.ContentRegion, n?.ToString()));
            }

            return Task.CompletedTask;
        }
    }
}