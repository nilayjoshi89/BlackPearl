using System.Runtime.Versioning;

using MahApps.Metro.Controls;

using Prism.Regions;

namespace BlackPearl.Mahapps
{
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public class HamburgerMenuSingleRegionAdapter : RegionAdapterBase<HamburgerMenu>
    {
        public HamburgerMenuSingleRegionAdapter(IRegionBehaviorFactory factory) : base(factory) { }
        protected override void Adapt(IRegion region, HamburgerMenu regionTarget)
        {
            region.ActiveViews.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
                && (e.NewItems?.Count ?? 0) > 0)
                {
                    regionTarget.Content = e.NewItems[0];
                }
            };
        }

        protected override IRegion CreateRegion() => new SingleActiveRegion();
    }
}
