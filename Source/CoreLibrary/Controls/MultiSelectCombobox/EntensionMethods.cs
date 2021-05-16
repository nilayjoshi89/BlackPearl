using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BlackPearl.Controls.CoreLibrary
{
    internal static class EntensionMethods
    {
        public static void ClearSelectionIfNoOperationInProgress(this ListBox suggestionList)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl)
                           || Keyboard.IsKeyDown(Key.RightCtrl)
                           || Keyboard.IsKeyDown(Key.LeftShift)
                           || Keyboard.IsKeyDown(Key.RightShift))
            {
                return;
            }

            suggestionList?.SelectedItems?.Clear();
        }
        public static IEnumerable<object> GetItemSource(this ListBox suggestionList) => suggestionList?.ItemsSource?.Cast<object>();
        public static void SetParagraphAsFirstBlock(this RichTextBox richTextBox, Paragraph paragraph)
        {
            if (richTextBox.Document.Blocks.FirstBlock == paragraph)
            {
                return;
            }

            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(paragraph);
        }
        public static string GetCurrentText(this RichTextBox richTextBox) => GetCurrentRunBlock(richTextBox)?.Text;
        public static void ResetCurrentText(this RichTextBox richTextBox)
        {
            Run runElement = GetCurrentRunBlock(richTextBox);
            if (runElement == null)
            {
                return;
            }

            runElement.Text = string.Empty;
        }
        public static void RemoveRunBlocks(this RichTextBox richTextBox)
        {
            var paragraph = richTextBox?.Document?.Blocks?.FirstOrDefault(b => b is Paragraph) as Paragraph;
            if (paragraph == null)
            {
                return;
            }

            var runTags = paragraph.Inlines?.Where(r => r is Run)?.ToList();
            if (runTags?.Any() != true)
            {
                return;
            }

            for (int i = 0; i < runTags.Count; i++)
            {
                paragraph?.Inlines?.Remove(runTags[i]);
            }
        }
        public static void SetCaretPositionToEnd(this RichTextBox richTextBox) => richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
        public static void TryFocus(this RichTextBox richTextBox)
        {
            try
            {
                if (richTextBox.Focusable)
                {
                    richTextBox.Focus();
                }
            }
            catch { }
        }

        private static Run GetCurrentRunBlock(this RichTextBox richTextBox) => richTextBox.CaretPosition.Parent as Run;
    }
}