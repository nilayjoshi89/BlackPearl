using Prism.Services.Dialogs;

namespace BlackPearl.Mahapps
{
    public class BlackPearlDialogWindow : BlackPearlMetroWindow, IDialogWindow
    {
        public IDialogResult Result { get; set; }
    }
}