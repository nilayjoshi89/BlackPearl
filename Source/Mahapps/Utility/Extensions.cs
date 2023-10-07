using System.Windows;

using BlackPearl.PrismUI;

using Prism.Ioc;

namespace BlackPearl.Mahapps
{
    public static class Extensions
    {
        public static IContainerRegistry RegisterBlackPearlServices(this IContainerRegistry registry)
        {
            registry.RegisterBlackPearlCoreServices()
                    .RegisterSingleton<IBlackPearlThemeManager, BlackPearlThemeManager>()
                    .Register<IBlackPearlThemeManager, BlackPearlThemeManager>();

            registry.RegisterForNavigation<ThemeView, ThemeViewModel>("BlackPearlThemeView");

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/BlackPearl.Mahapps;component/Resources/DefaultStyle.xaml") });

            return registry;
        }
    }
}