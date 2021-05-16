using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using BlackPearl.Controls.Contract;

namespace BlackPearl.Controls.CoreLibrary
{
    internal static class EntensionMethods
    {
        #region ListBox
        public static void ClearSelection(this ListBox suggestionList, Func<bool> precondition = null)
        {
            if(precondition != null
                && !precondition())
            {
                return;
            }

            //if (Keyboard.IsKeyDown(Key.LeftCtrl)
            //               || Keyboard.IsKeyDown(Key.RightCtrl)
            //               || Keyboard.IsKeyDown(Key.LeftShift)
            //               || Keyboard.IsKeyDown(Key.RightShift))
            //{
            //    return;
            //}

            suggestionList?.SelectedItems?.Clear();
        }
        //public static void ClearSelection(this ListBox suggestionList) => suggestionList?.SelectedItems?.Clear();
        public static IEnumerable<object> GetItemSource(this ListBox suggestionList) => suggestionList?.ItemsSource?.Cast<object>();
        #endregion

        #region RichTextBox
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
            var paragraph = richTextBox?.Document?.Blocks?.FirstBlock as Paragraph;
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
        public static void TryFocus(this RichTextBox richTextBox)
        {
            try
            {
                if (richTextBox?.Focusable == true)
                {
                    richTextBox.Focus();
                }
            }
            catch { }
        }
        public static void SetupOrCheckParagraph(this RichTextBox richTextBox)
        {
            try
            {
                if (richTextBox?.Document?.Blocks?.FirstBlock is Paragraph)
                {
                    return;
                }

                var paragraph = new Paragraph() { Style = new Style() };
                richTextBox.Document.Blocks.Clear();
                richTextBox.Document.Blocks.Add(paragraph);
            }
            catch { }
        }
        public static void AddToParagraph(this RichTextBox richTextBox, object itemToAdd, Func<object, InlineUIContainer> createInlineElementFunct)
        {
            try
            {
                Paragraph paragraph = richTextBox?.GetParagraph();
                if (paragraph == null)
                {
                    return;
                }

                InlineUIContainer elementToAdd = createInlineElementFunct(itemToAdd);

                if (paragraph.Inlines.FirstInline == null)
                {
                    //First element to insert
                    paragraph.Inlines.Add(elementToAdd);
                }
                else
                {
                    //Insert at the end
                    paragraph.Inlines.InsertAfter(paragraph.Inlines.LastInline, elementToAdd);
                }

                richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
            }
            catch { }
        }
        public static void ClearParagraph(this RichTextBox richTextBox) => richTextBox?.GetParagraph()?.Inlines?.Clear();

        private static Run GetCurrentRunBlock(this RichTextBox richTextBox) => richTextBox?.CaretPosition?.Parent as Run;
        private static Paragraph GetParagraph(this RichTextBox richTextBox) => richTextBox?.Document?.Blocks?.FirstBlock as Paragraph;
        #endregion

        #region Popup
        public static void Show(this Popup popupElement, Func<bool> precondition, Action postAction) => ShowHide(popupElement, precondition, postAction, true);
        public static void Hide(this Popup popupElement, Func<bool> precondition, Action postAction) => ShowHide(popupElement, precondition, postAction, false);

        private static void ShowHide(this Popup popupElement, Func<bool> precondition, Action postAction, bool valueToSet)
        {
            try
            {
                if (popupElement == null
                    || popupElement.IsOpen == valueToSet)
                {
                    return;
                }

                bool proceed = precondition == null || precondition();
                if (!proceed)
                {
                    return;
                }

                popupElement.IsOpen = valueToSet;
                postAction?.Invoke();
            }
            catch { }
        }
        #endregion

        #region Lookup Contract
        public static bool HasAnyExactMatch(this IEnumerable<object> source, string itemString, ILookUpContract contract, object sender)
            => source?.Any(i => contract?.IsItemEqualToString(sender, i, itemString) == true) == true;

        public static object GetExactMatch(this IEnumerable<object> source, string itemString, ILookUpContract contract, object sender)
            => source?.FirstOrDefault(i => contract?.IsItemEqualToString(sender, i, itemString) == true);

        public static IEnumerable<object> GetSuggestions(this IEnumerable<object> source, string itemString, ILookUpContract contract, object sender)
            => source?.Where(i => contract?.IsItemMatchingSearchString(sender, i, itemString) == true);
        #endregion
    }
}