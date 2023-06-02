using MahApps.Metro.Controls;
using BlackPearl.Controls.Demo.ViewModels;

namespace BlackPearl.Controls.Demo.Views
{
    public partial class AdvancedExampleView : MetroContentControl
    {
        public AdvancedExampleView()
        {
            InitializeComponent();
            DataContext = new AdvancedExampleViewModel();
        }
    }
}