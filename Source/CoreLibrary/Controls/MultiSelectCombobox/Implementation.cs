using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using BlackPearl.Controls.Extension;

namespace BlackPearl.Controls.CoreLibrary
{
    public sealed partial class MultiSelectCombobox
    {
        #region Members
        private bool areHandlersRegistered = true;
        private readonly object handlerLock = new object();
        private Paragraph paragraph = null;
        private int selectionStart = -1, suggestionIndex = -1;
        #endregion

        #region Control Event Handlers
        private void SuggestionElement_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                TrySetSelectedItemFromSuggestionDropDown();
                TrySettingFocusToRichTextBox();
            }
        }
        private void SuggestionElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SuggestionElement.SelectedItems.Count > 0 && Keyboard.Modifiers != ModifierKeys.Control)
            {
                TrySetSelectedItemFromSuggestionDropDown();
                TrySettingFocusToRichTextBox();
            }
        }
        private void MultiSelectCombobox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //If DropDown has focus, return
                if (PopupElement.IsKeyboardFocusWithin)
                {
                    return;
                }

                //Remove all invalid texts from
                RemoveInvalidTexts();

                //Hide drop-down
                HideSuggestionDropDown();
            }
            catch { }
        }
        private void MultiSelectCombobox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //User can remove paragraph reference by 'Select all & delete' in RichTextBox
                //Following method call with make sure local paragraph remains part of RichTextBox
                ReInitializeParagraphIfRequired();

                switch (e.Key)
                {
                    case Key.Down:
                        {
                            e.Handled = true;

                            //Open suggestion drop down for entered
                            //Or Increment selection index in drop-down

                            //Get current text
                            string currentText = GetUserEnteredText();

                            //Check if any suggestion available for given text
                            if (!HasAnyItemStartingWith(ItemSource?.Cast<object>(), currentText))
                            {
                                //No suggestion to show, return
                                return;
                            }

                            //Show drop-down
                            ShowSuggestionDropDown();

                            //If multi-selection
                            if (Keyboard.Modifiers == ModifierKeys.Shift)
                            {
                                DoDownwardMultiSelection();
                                break;
                            }

                            //Reset multi-select flag
                            selectionStart = -1;

                            //Increment selected item index in drop-down
                            IncrementSelectedIndex();
                        }
                        break;
                    case Key.Up:
                        {
                            e.Handled = true;

                            //Get current text
                            string currentText = GetUserEnteredText();
                            //Check if any suggestion available for given text
                            if (!HasAnyItemStartingWith(ItemSource?.Cast<object>(), currentText))
                            {
                                //No suggestion to show, return
                                return;
                            }
                            //Show drop-down
                            ShowSuggestionDropDown();

                            //If multi-select
                            if (Keyboard.Modifiers == ModifierKeys.Shift)
                            {
                                DoUpwardMultiSelection();
                                break;
                            }

                            //Reset multi-select flag
                            selectionStart = -1;

                            //Decrement selected item index in drop-down
                            DecrementSelectedIndex();
                        }
                        break;
                    case Key.Enter:
                        {
                            e.Handled = true;

                            //Try select item based on selection in Suggestion drop-down
                            TrySetSelectedItemFromSuggestionDropDown();

                            TrySettingFocusToRichTextBox();
                        }
                        break;
                    case Key.Escape:
                        {
                            e.Handled = true;

                            //Hide suggestion drop-down
                            HideSuggestionDropDown();

                            TrySettingFocusToRichTextBox();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }
        private void RichTextBoxElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Unsubscribe handlers first
                if (!UnsubscribeHandlers())
                {
                    //Failed to unsubscribe, return
                    return;
                }

                //All text entered in Control goes to Run element of RichTextBox
                //If Current caret position is in run element
                if (RichTextBoxElement.CaretPosition.Parent is Run runTag)
                {
                    //if (SearchTextStyle != null
                    //    && runTag.Style != SearchTextStyle)
                    //{
                    //    runTag.Style = SearchTextStyle;
                    //}

                    //Check if current text ends with ItemSeparator or not
                    if (runTag.Text.EndsWith(ItemSeparator.ToString()))
                    {
                        //User is expecting to complete item selection
                        //Check if current text is blank + separator
                        if (string.IsNullOrEmpty(runTag.Text.Trim(ItemSeparator)))
                        {
                            //there's nothing to select
                            //set current text to empty
                            runTag.Text = string.Empty;
                        }
                        //User has entered valid text + separator
                        else
                        {
                            //Try select item from source based on current entered text
                            //Set force-add to true as user is confident about selected text
                            TrySetSelectedItem(runTag, forceAdd: true);
                        }
                    }
                    //else user is still expected to make changes to current text
                }

                //Show/Hide pop-up based on selected-item
                ShowHideSuggestionDropDown();
            }
            catch { }
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Subscribes to events for controls
        /// </summary>
        private void SubsribeHandlers()
        {
            //Check handler registration
            if (areHandlersRegistered)
            {
                //if already registered, return
                return;
            }

            //acquire a lock
            lock (handlerLock)
            {
                //double check registration
                if (areHandlersRegistered)
                {
                    //race condition, return
                    return;
                }

                //set handler flag to true
                areHandlersRegistered = true;

                //subscribe
                SuggestionElement.PreviewMouseUp += SuggestionElement_PreviewMouseUp;
                SuggestionElement.PreviewKeyUp += SuggestionElement_PreviewKeyUp;
                RichTextBoxElement.TextChanged += RichTextBoxElement_TextChanged;
                PreviewKeyDown += MultiSelectCombobox_PreviewKeyDown;
                LostFocus += MultiSelectCombobox_LostFocus;
            }
        }
        /// <summary>
        /// Unsubscribes to events for controls
        /// </summary>
        private bool UnsubscribeHandlers()
        {
            //Check handler registration
            if (!areHandlersRegistered)
            {
                //If already unsubscribed, return
                return false;
            }

            //acquire a lock
            lock (handlerLock)
            {
                //double check registration
                if (!areHandlersRegistered)
                {
                    //race condition, return
                    return false;
                }

                //set handler registration flag
                areHandlersRegistered = false;

                //unsubscribe
                SuggestionElement.PreviewMouseUp -= SuggestionElement_PreviewMouseUp;
                SuggestionElement.PreviewKeyUp -= SuggestionElement_PreviewKeyUp;
                RichTextBoxElement.TextChanged -= RichTextBoxElement_TextChanged;
                PreviewKeyDown -= MultiSelectCombobox_PreviewKeyDown;
                LostFocus -= MultiSelectCombobox_LostFocus;

                return true;
            }
        }
        /// <summary>
        /// Shows or Hides Suggestion drop-down based on current text
        /// </summary>
        private void ShowHideSuggestionDropDown()
        {
            //Get current entered text
            string currentText = GetUserEnteredText();
            //Check if there are any items to be shown in suggestion drop-down for given text
            bool hasAnySuggestion = HasAnyItemStartingWith(ItemSource?.Cast<object>(), currentText);
            //Check for suggestion
            if (!hasAnySuggestion)
            {
                //No suggestion found
                HideSuggestionDropDown();
                return;
            }

            //Items found for suggestion drop-down
            //Get Items to be shown in suggestion drop-down for current text
            IEnumerable<object> itemsToAdd = GetItemsStartingWith(ItemSource?.Cast<object>(), currentText);

            //Double check, if there are items to display
            if (itemsToAdd?.Any() != true)
            {
                //No suggestion found
                HideSuggestionDropDown();
                return;
            }

            //Add suggestion items to suggestion drop-down
            suggestionElement.ItemsSource = itemsToAdd;

            //Show suggestion drop-down
            ShowSuggestionDropDown();
        }
        /// <summary>
        /// Shows suggestion drop-down
        /// </summary>
        private void ShowSuggestionDropDown()
        {
            //Opens drop-down if not already
            if (!PopupElement.IsOpen)
            {
                suggestionIndex = -1;
                selectionStart = -1;
                PopupElement.IsOpen = true;
            }
        }
        /// <summary>
        /// Hides suggestion drop-down
        /// </summary>
        private void HideSuggestionDropDown()
        {
            //Closes drop-down if not already
            if (PopupElement.IsOpen)
            {
                PopupElement.IsOpen = false;
                suggestionIndex = -1;
                selectionStart = -1;
            }
        }
        /// <summary>
        /// Re-initializes paragraph ref in RichTextBox
        /// </summary>
        private void ReInitializeParagraphIfRequired()
        {
            if (RichTextBoxElement.Document.Blocks.FirstBlock != paragraph)
            {
                RichTextBoxElement.Document.Blocks.Clear();
                RichTextBoxElement.Document.Blocks.Add(paragraph);
            }
        }
        /// <summary>
        /// Gets user entered text in RichTextBox
        /// </summary>
        /// <returns></returns>
        private string GetUserEnteredText() => (RichTextBoxElement.CaretPosition.Parent as Run)?.Text;
        /// <summary>
        /// Increments selection index for suggestion drop-down
        /// </summary>
        private void IncrementSelectedIndex()
        {
            var suggestionItemSource = suggestionElement.ItemsSource.Cast<object>();
            int totalCount = suggestionItemSource.Count();
            suggestionIndex = (suggestionIndex + 1 >= totalCount)
                                ? totalCount - 1
                                : suggestionIndex + 1;

            //Clear any previous selection
            SuggestionElement.SelectedItems.Clear();
            SuggestionElement.SelectedItems.Add(suggestionItemSource.ElementAt(suggestionIndex));
        }
        /// <summary>
        /// Decrement selection index for suggestion drop-down
        /// </summary>
        private void DecrementSelectedIndex()
        {
            var suggestionItemSource = suggestionElement.ItemsSource.Cast<object>();
            suggestionIndex = suggestionIndex < 1
                ? 0
                : suggestionIndex - 1;

            //Clear any previous selection
            SuggestionElement.SelectedItems.Clear();
            SuggestionElement.SelectedItems.Add(suggestionItemSource.ElementAt(suggestionIndex));
        }
        /// <summary>
        /// Downward selection for suggestion drop-down
        /// </summary>
        private void DoDownwardMultiSelection()
        {
            var suggestionItemSource = suggestionElement.ItemsSource.Cast<object>();
            int oldIndex = suggestionIndex;
            int totalCount = suggestionItemSource.Count();
            suggestionIndex = (suggestionIndex + 1 >= totalCount)
                                ? totalCount - 1
                                : suggestionIndex + 1;

            //If its boundary, return
            if (oldIndex == suggestionIndex)
            {
                return;
            }

            //If its first time - Start of selection
            if (selectionStart == -1)
            {
                //set previous index as selection start
                selectionStart = oldIndex;
                //Add current item to selected items list
                SuggestionElement.SelectedItems.Add(suggestionItemSource.ElementAt(suggestionIndex));
                return;
            }

            //If selection is shrinking then remove previous selected element
            if (selectionStart > oldIndex)
            {
                SuggestionElement.SelectedItems.Remove(suggestionItemSource.ElementAt(oldIndex));
                return;
            }

            //Otherwise, selection is growing, add current element to selected items list
            SuggestionElement.SelectedItems.Add(suggestionItemSource.ElementAt(suggestionIndex));
        }
        /// <summary>
        /// Upward selection for suggestion drop-down
        /// </summary>
        private void DoUpwardMultiSelection()
        {
            int oldIndex = suggestionIndex;
            suggestionIndex = suggestionIndex < 1
                ? 0
                : suggestionIndex - 1;

            //If its boundary, return
            if (oldIndex == suggestionIndex)
            {
                return;
            }

            var suggestionItemSource = suggestionElement.ItemsSource.Cast<object>();
            //If its first time - Start of selection
            if (selectionStart == -1)
            {
                //set previous index as selection start
                selectionStart = oldIndex;
                //Add current item to selected items list
                SuggestionElement.SelectedItems.Add(suggestionItemSource.ElementAt(suggestionIndex));
                return;
            }

            //If selection is shrinking then remove previous selected element
            if (selectionStart < oldIndex)
            {
                SuggestionElement.SelectedItems.Remove(suggestionItemSource.ElementAt(oldIndex));
                return;
            }

            //Otherwise, selection is growing, add current element to selected items list
            SuggestionElement.SelectedItems.Add(suggestionItemSource.ElementAt(suggestionIndex));
        }
        /// <summary>
        /// Tries to set item from entered text in RichTextBox
        /// </summary>
        /// <param name="runElementToRemove">Run element in which text is entered</param>
        /// <param name="forceAdd">Allows creation of new item</param>
        private void TrySetSelectedItem(Run runElementToRemove = null, bool forceAdd = false)
        {
            //Trim text of item separator and blank space
            string itemString = runElementToRemove?.Text.Trim(ItemSeparator, ' ');

            //Hide suggestion drop-down
            HideSuggestionDropDown();

            //Remove Run element from RichTextBox
            if (runElementToRemove != null)
            {
                paragraph.Inlines.Remove(runElementToRemove);
            }

            //If selected text is not forced to add or LookUpContract supports creation of new item
            //Default LookUpContract implementation does not support item creation
            if (!forceAdd || LookUpContract?.SupportsNewObjectCreation != true)
            {
                //Check if entered text is valid item or not
                if (!HasAnyItem(ItemSource?.Cast<object>(), itemString))
                {
                    //Not a valid item, return
                    return;
                }
            }

            //item text is a valid in ItemSource or can be created using LookUpContract

            //Check if item is already selected or not
            if (HasAnyItem(SelectedItems?.Cast<object>(), itemString))
            {
                //already selected, return
                return;
            }

            //Try to get item from source based on itemString
            object itemToAdd = GetItemFromSource(ItemSource?.Cast<object>(), itemString);

            if (itemToAdd == null && LookUpContract?.SupportsNewObjectCreation == true)
            {
                //If item is not found in ItemSource look-up and LookUpContract supports creation of new item
                itemToAdd = LookUpContract.CreateObject(this, itemString);
            }

            //If item is available
            if (itemToAdd != null)
            {
                //Add item to Selected Item list
                SelectedItems?.Add(itemToAdd);
                //Add item in RichTextBox UI
                AddItemToUI(itemToAdd);

                RaiseSelectionChangedEvent(new ArrayList(0), new[] { itemToAdd });
            }
        }
        /// <summary>
        /// Gets item from source for given text
        /// </summary>
        /// <param name="source">Item source to search</param>
        /// <param name="textToSearch">text to search</param>
        /// <returns>Item from source if any else null</returns>
        private object GetItemFromSource(IEnumerable<object> source, string textToSearch) =>
            //Uses LookUpContract to get exact item from source
            source?.FirstOrDefault(i => LookUpContract?.IsItemEqualToString(this, i, textToSearch) == true);
        /// <summary>
        /// Adds new item to RichTextBox and sets caret position to end
        /// </summary>
        /// <param name="itemToAdd"></param>
        private void AddItemToUI(object itemToAdd)
        {
            //Get new element to add in RichTextBox
            InlineUIContainer itemToAddUIElement = CreateInlineUIElement(itemToAdd);

            if (paragraph.Inlines.FirstInline == null)
            {
                //First element to insert
                paragraph.Inlines.Add(itemToAddUIElement);
            }
            else
            {
                //Insert at the end
                paragraph.Inlines.InsertAfter(paragraph.Inlines.LastInline, itemToAddUIElement);
            }

            //Set caret position to the end
            RichTextBoxElement.CaretPosition = RichTextBoxElement.CaretPosition.DocumentEnd;
        }
        /// <summary>
        /// Create RichTextBox document element for given object
        /// </summary>
        /// <param name="objectToDisplay"></param>
        /// <returns></returns>
        private InlineUIContainer CreateInlineUIElement(object objectToDisplay)
        {
            var tb = new TextBlock()
            {
                //Text based on Display member path
                Text = objectToDisplay.GetPropertyValue(DisplayMemberPath)?.ToString() + ItemSeparator,
                //Set object in Tag for easy access for future operations
                Tag = objectToDisplay
            };

            tb.Unloaded += Tb_Unloaded;
            return new InlineUIContainer(tb);
        }
        /// <summary>
        /// Event to handle scenario where User removes selected item from UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsLoaded
                    || !(sender is TextBlock tb))
                {
                    return;
                }

                tb.Unloaded -= Tb_Unloaded;
                SelectedItems?.Remove(tb.Tag);
                RaiseSelectionChangedEvent(new[] { tb.Tag }, new ArrayList(0));
            }
            catch { }
        }
        /// <summary>
        /// Tries to set item from suggestion drop-down
        /// </summary>
        /// <param name="runTagToRemove"></param>
        /// <param name="itemObject"></param>
        private void TrySetSelectedItemFromSuggestionDropDown()
        {
            try
            {
                //Unsubscribe handlers first
                if (!UnsubscribeHandlers())
                {
                    //Failed to unsubscribe, return
                    return;
                }

                //Check if drop down is open or has any item selected
                if (!PopupElement.IsOpen || SuggestionElement.SelectedItems.Count < 1)
                {
                    return;
                }

                //Hide drop-down
                HideSuggestionDropDown();

                //Remove any user entered text if any
                if (RichTextBoxElement.CaretPosition.Parent is Run runElementToRemove)
                {
                    paragraph.Inlines.Remove(runElementToRemove);
                }

                foreach (object itemObject in SuggestionElement.SelectedItems)
                {
                    //Check if item is already selected or not
                    if (SelectedItems?.Contains(itemObject) == true)
                    {
                        continue;
                    }

                    //Add item to selected item list
                    SelectedItems?.Add(itemObject);
                    //Add item to UI
                    AddItemToUI(itemObject);
                }

                RaiseSelectionChangedEvent(new ArrayList(0), SuggestionElement.SelectedItems);

                SuggestionElement.SelectedItems?.Clear();
                SuggestionElement.ItemsSource = ItemSource;
            }
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }
        }
        /// <summary>
        /// Tries to set focus on RichTextBox
        /// </summary>
        private void TrySettingFocusToRichTextBox()
        {
            try
            {
                if (RichTextBoxElement.Focusable)
                {
                    RichTextBoxElement.Focus();
                }
            }
            catch { }
        }
        /// <summary>
        /// Removes all invalid texts from RichTextBox except selected item
        /// </summary>
        private void RemoveInvalidTexts()
        {
            try
            {
                //Unsubscribe handlers first
                if (!UnsubscribeHandlers())
                {
                    //Failed to unsubscribe, return
                    return;
                }
                var runTags = paragraph?.Inlines?.Where(r => r is Run).ToList();

                for (int i = 0; i < runTags.Count; i++)
                {
                    paragraph?.Inlines?.Remove(runTags[i]);
                }
            }
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }
        }

        #region LookUp Methods
        /// <summary>
        /// Gets all items from source matching search criteria
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="textToSearch">search text</param>
        /// <returns>All items fulfilling search criteria</returns>
        private IEnumerable<object> GetItemsStartingWith(IEnumerable<object> source, string textToSearch) => source?.Where(i => LookUpContract?.IsItemMatchingSearchString(this, i, textToSearch) == true);
        /// <summary>
        /// Checks for any source item which match search criteria
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="textToSearch">search text</param>
        /// <returns>true if any item found else false</returns>
        private bool HasAnyItemStartingWith(IEnumerable<object> source, string textToSearch) => source?.Any(i => LookUpContract?.IsItemMatchingSearchString(this, i, textToSearch) == true) == true;
        /// <summary>
        /// Checks source for item having equivalent value as textToSearch
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="textToSearch">search text</param>
        /// <returns></returns>
        private bool HasAnyItem(IEnumerable<object> source, string textToSearch) => source?.Any(i => LookUpContract?.IsItemEqualToString(this, i, textToSearch) == true) == true;
        #endregion

        #endregion
    }
}