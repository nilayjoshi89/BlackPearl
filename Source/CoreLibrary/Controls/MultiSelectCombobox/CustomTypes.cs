using System.Windows;
using System.Windows.Controls;

namespace BlackPearl.Controls.CoreLibrary
{
    public class MultiSelectCombobox_CustomRichTextBox : RichTextBox
    {
        //disable default OnDrop event witch give " " strings...
        protected override void OnDrop(DragEventArgs e) { }
    }
}