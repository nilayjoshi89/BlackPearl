using MahApps.Metro.Controls;
using BlackPearl.Controls.Demo.ViewModels;

namespace BlackPearl.Controls.Demo.Views
{
    public partial class SimpleExampleView : MetroContentControl
    {
        public SimpleExampleView()
        {
            InitializeComponent();
            DataContext = new SimpleExampleViewModel();
        }
    }
}