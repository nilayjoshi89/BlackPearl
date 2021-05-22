using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

using BlackPearl.Controls.Contract;
using BlackPearl.Controls.CoreLibrary.Behavior;

namespace BlackPearl.Controls.CoreLibrary
{
    internal static class EntensionMethods
    {
        #region ListBox
        public static void ClearSelection(this ListBox suggestionList, Func<bool> precondition = null)
        {
            if (precondition != null
                && !precondition())
            {
                return;
            }

            suggestionList?.SelectedItems?.Clear();
        }
        public static IEnumerable<object> GetItemSource(this ListBox suggestionList) => suggestionList?.ItemsSource?.Cast<object>();
        public static void SelectNextItem(this ListBox suggestionList) => suggestionList.SingleItemSelection(1);
        public static void SelectPreviousItem(this ListBox suggestionList) => suggestionList.SingleItemSelection(-1);
        public static void SelectMultipleNextItem(this ListBox suggestionList) => suggestionList.SelectMultipleItem(1);
        public static void SelectMultiplePreviousItem(this ListBox suggestionList) => suggestionList.SelectMultipleItem(-1);
        public static int GetSelectionStart(this ListBox suggestionList)
        {
            if (suggestionList == null)
            {
                return -1;
            }

            return ListBoxAttachedProperties.GetSelectionStartIndex(suggestionList);
        }
        public static void SetSelectionStart(this ListBox suggestionList, int index)
        {
            if (suggestionList == null)
            {
                return;
            }

            ListBoxAttachedProperties.SetSelectionStartIndex(suggestionList, index);
        }
        public static int GetSelectionEnd(this ListBox suggestionList)
        {
            if (suggestionList == null)
            {
                return -1;
            }

            return ListBoxAttachedProperties.GetSelectionEndIndex(suggestionList);
        }
        public static void SetSelectionEnd(this ListBox suggestionList, int index)
        {
            if (suggestionList == null)
            {
                return;
            }

            ListBoxAttachedProperties.SetSelectioEndtIndex(suggestionList, index);
        }
        public static void CleanOperation(this ListBox suggestionList, SuggestionCleanupOperation operation, IEnumerable goldenItemSource)
        {
            if ((operation & SuggestionCleanupOperation.ClearSelection) == SuggestionCleanupOperation.ClearSelection)
            {
                suggestionList.ClearSelection();
            }

            if ((operation & SuggestionCleanupOperation.ResetIndex) == SuggestionCleanupOperation.ResetIndex)
            {
                suggestionList.SetSelectionStart(-1);
                suggestionList.SetSelectionEnd(-1);
            }

            if ((operation & SuggestionCleanupOperation.ResetItemSource) == SuggestionCleanupOperation.ResetItemSource)
            {
                suggestionList.ItemsSource = goldenItemSource;
            }
        }

        private static void SingleItemSelection(this ListBox suggestionList, int delta)
        {
            if (suggestionList == null
                || (suggestionList.SelectedIndex + delta) < 0)
            {
                return;
            }

            suggestionList.SelectedIndex += delta;

            suggestionList.SetSelectionStart(-1);
            suggestionList.SetSelectionEnd(-1);
        }
        private static void SelectMultipleItem(this ListBox suggestionList, int delta)
        {
            if (suggestionList == null)
            {
                return;
            }

            ItemCollection suggestionItemSource = suggestionList.Items;
            int selectionStart = suggestionList.GetSelectionStart();
            int selectionEnd = suggestionList.GetSelectionEnd();
            int totalItemsCount = suggestionItemSource.Count;

            //If its first time - Start of selection
            if (selectionStart == -1 || selectionEnd == -1)
            {
                selectionStart = suggestionList.SelectedIndex;
                selectionEnd = suggestionList.SelectedIndex + delta;
                selectionEnd = (selectionEnd < 0)
                                ? 0
                                : (selectionEnd >= totalItemsCount)
                                ? totalItemsCount - 1
                                : selectionEnd;

                suggestionList.SetSelectionStart(selectionStart);
                suggestionList.SetSelectionEnd(selectionEnd);

                //Add current item to selected items list
                suggestionList.SelectedItems.Add(suggestionItemSource[selectionEnd]);
                return;
            }


            int newIndex = selectionEnd + delta;
            newIndex = (newIndex < 0)
                               ? 0
                               : (newIndex >= totalItemsCount)
                               ? totalItemsCount - 1
                               : newIndex;

            //If its boundary, return
            if (selectionEnd == newIndex)
            {
                return;
            }

            //If selection is shrinking then remove previous selected element
            if ((selectionStart > selectionEnd && newIndex > selectionEnd)
                || (selectionStart < selectionEnd && newIndex < selectionEnd))
            {
                suggestionList.SelectedItems.Remove(suggestionItemSource[selectionEnd]);
                suggestionList.SetSelectionEnd(newIndex);
                return;
            }

            //Otherwise, selection is growing, add current element to selected items list
            suggestionList.SetSelectionEnd(newIndex);
            suggestionList.SelectedItems.Add(suggestionItemSource[newIndex]);
        }

        internal enum SuggestionCleanupOperation
        {
            ResetIndex = 1,
            ClearSelection = 2,
            ResetItemSource = 4
        };
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
        public static void SetParagraphAsFirstBlock(this RichTextBox richTextBox)
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