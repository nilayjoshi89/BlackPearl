using System.Collections.ObjectModel;

using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace BlackPearl.Controls.Demo
{
    public static class Constants
    {
        public static string ThemeView = "BlackPearlThemeView";
        public static string MultiSelectComboBoxView = "MultiSelectComboBoxView";
        public static string ContentRegion = "ContentRegion";

        public static readonly ObservableCollection<HamburgerMenuIconItem> DefaultMenuItems = new ObservableCollection<HamburgerMenuIconItem>()
        {
            new HamburgerMenuIconItem()
            {
                Icon = new PackIconBootstrapIcons() { Kind = PackIconBootstrapIconsKind.Tools, Height = 25, Width = 25 },
                Label = "Controls",
                CommandParameter = MultiSelectComboBoxView,
                ToolTip = "Controls",
            },
            new HamburgerMenuIconItem()
            {
                Icon = new PackIconEvaIcons() { Kind = PackIconEvaIconsKind.ColorPaletteOutline, Height = 25, Width = 25 },
                Label = "Theme",
                CommandParameter = ThemeView,
                ToolTip = "Theme",
            }
        };
    }
}
