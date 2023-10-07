using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using BlackPearl.PrismUI;

using Prism.Commands;
using Prism.Events;


namespace BlackPearl.Mahapps
{
    public class ThemeViewModel : BlackPearlViewModel
    {
        #region Members
        private readonly IBlackPearlThemeManager themeManager;
        private readonly IDispatcherService dispatcherService;
        private readonly IEventAggregator eventAggregator;
        private bool isDark;
        private AccentInfo selectedAccent;
        #endregion

        #region Constructor
        public ThemeViewModel(IBlackPearlThemeManager themeManager, IDispatcherService dispatcherService, IEventAggregator eventAggregator)
        {
            this.themeManager = themeManager;
            this.dispatcherService = dispatcherService;
            this.eventAggregator = eventAggregator;
        }
        #endregion

        #region Properties
        public ObservableCollection<AccentInfo> Accents { get; set; } = new ObservableCollection<AccentInfo>();
        public AccentInfo SelectedAccent
        {
            get => selectedAccent;
            set
            {
                selectedAccent = value;
                AccentChanged();
            }
        }
        public string ThemeText => IsDark ? "Dark" : "Light";
        public Visibility MoonVisibility => IsDark ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SunVisibility => !IsDark ? Visibility.Visible : Visibility.Collapsed;
        public DelegateCommand IncreaseFontCommand => new DelegateCommand(IncreaseFont);
        public DelegateCommand DecreaseFontCommand => new DelegateCommand(DecreaseFont);
        public bool IsDark
        {
            get => isDark;
            set
            {
                isDark = value;
                IsDarkChanged();
            }
        }
        #endregion

        #region Methods
        public override async Task OnLoad()
        {
            try
            {
                var allAccentInfo = AccentInfo.GetAll();
                var accentColor = themeManager.GetAccentColor();
                isDark = themeManager.IsDarkTheme();
                var accentInfo = allAccentInfo.FirstOrDefault(a => a.Value == accentColor);

                await dispatcherService.Execute(() =>
                {
                    Accents.AddRange(allAccentInfo);
                    selectedAccent = accentInfo;

                    RaisePropertyChanged(nameof(SelectedAccent));
                    RaisePropertyChanged(nameof(IsDark));
                    RaisePropertyChanged(nameof(ThemeText));
                    RaisePropertyChanged(nameof(MoonVisibility));
                    RaisePropertyChanged(nameof(SunVisibility));
                });
            }
            catch { }
        }
        private void DecreaseFont()
        {
            try
            {
                themeManager.DecreaseFontSize();
            }
            catch { }
        }
        private void IncreaseFont()
        {
            try
            {
                themeManager.IncreaseFontSize();
            }
            catch { }
        }
        private void AccentChanged()
        {
            try
            {
                themeManager.SetTheme(SelectedAccent.Value, IsDark);
            }
            catch { }
        }
        private void IsDarkChanged()
        {
            try
            {
                themeManager.SetTheme(SelectedAccent.Value, IsDark);
                RaisePropertyChanged(nameof(ThemeText));
                RaisePropertyChanged(nameof(MoonVisibility));
                RaisePropertyChanged(nameof(SunVisibility));
            }
            catch { }
        }
        #endregion
    }

    public class AccentInfo
    {
        public string Name { get; set; }
        public Color Value { get; set; }
        public Brush Brush => new SolidColorBrush(Value);
        public static List<AccentInfo> GetAll()
            => typeof(Colors).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty)
                .Select(p => new AccentInfo()
                {
                    Name = p.Name,
                    Value = (Color)p.GetValue(null)
                }).ToList();
    }
}
