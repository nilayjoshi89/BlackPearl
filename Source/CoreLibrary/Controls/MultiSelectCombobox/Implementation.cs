using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using BlackPearl.Controls.Extension;

using EM = BlackPearl.Controls.CoreLibrary.EntensionMethods;

namespace BlackPearl.Controls.CoreLibrary
{
    public sealed partial class MultiSelectCombobox
    {
        #region Members
        private bool isHandlerRegistered = true;
        private readonly object handlerLock = new object();
        #endregion

        #region Control Event Handlers
        /// <summary>
        /// Suggestion drop down - key board key up
        /// Forces control to update selected item based on selection in suggestion drop down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestionElement_PreviewKeyUp(object sender, KeyEventArgs e) => UpdateSelectedItemIfSelectionIsDone(e.Key);
        private void SuggestionDropdown_PreviewMouseDown(object sender, MouseButtonEventArgs e) => SuggestionElement.ClearSelection(IsSelectionProcessCompleted);
        /// <summary>
        /// Suggestion drop down - mouse key up
        /// Forces control to update selected item based on selection in suggestion drop down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestionDropdown_PreviewMouseUp(object sender, MouseButtonEventArgs e) => UpdateSelectedItemIfSelectionIsDone();
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
                HideSuggestions(EM.SuggestionCleanupOperation.ResetIndex | EM.SuggestionCleanupOperation.ClearSelection);
            }
            catch { }
        }
        private void MultiSelectCombobox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //User can remove paragraph reference by 'Select all & delete' in RichTextBox
                //Following method call with make sure local paragraph remains part of RichTextBox
                RichTextBoxElement.SetParagraphAsFirstBlock();

                switch (e.Key)
                {
                    case Key.Down:
                        {
                            e.Handled = true;
                            HandleKeyboardDownKeyPress();
                        }
                        break;
                    case Key.Up:
                        {
                            e.Handled = true;
                            HandleKeyboardUpKeyPress();
                        }
                        break;
                    case Key.Enter:
                        {
                            e.Handled = true;
                            UpdateSelectedItemsFromSuggestionDropdown();
                        }
                        break;
                    case Key.Escape:
                        {
                            e.Handled = true;
                            HideSuggestions(EM.SuggestionCleanupOperation.ResetIndex | EM.SuggestionCleanupOperation.ClearSelection);
                            RichTextBoxElement.TryFocus();
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
                //All text entered in Control goes to Run element of RichTextBox
                string userEnteredText = RichTextBoxElement.GetCurrentText();
                if (!IsEndOfTextDetected(userEnteredText))
                {
                    UpdateSuggestionAndShowHideDropDown(userEnteredText);
                    return;
                }

                if (!UnsubscribeHandler())
                {
                    return;
                }

                //Hide suggestion drop-down
                //Reset suggestion drop down list
                HideSuggestions(EM.SuggestionCleanupOperation.ResetIndex | EM.SuggestionCleanupOperation.ResetItemSource);

                //User is expecting to complete item selection
                if (IsBlankTextWithItemSeparator(userEnteredText))
                {
                    //there's nothing to select
                    //set current text to empty
                    RichTextBoxElement.ResetCurrentText();
                    return;
                }

                //User has entered valid text + separator
                RichTextBoxElement.RemoveRunBlocks();
                //Try select item from source based on current entered text
                UpdateSelectedItemsFromEnteredText(userEnteredText);
            }
            catch { }
            finally
            {
                //Subscribe back
                SubsribeHandler();
            }
        }
        #endregion

        #region Methods

        #region event handler helper methods
        private bool IsBlankTextWithItemSeparator(string userEnteredText) => string.IsNullOrWhiteSpace(userEnteredText.Trim(ItemSeparator));
        private bool IsEndOfTextDetected(string userEnteredText) => !string.IsNullOrEmpty(userEnteredText) && userEnteredText.EndsWith(ItemSeparator.ToString());
        private void UpdateSuggestionAndShowHideDropDown(string userEnteredText)
        {
            bool hasAnySuggestionToShow = UpdateSuggestions(userEnteredText);
            if (hasAnySuggestionToShow)
            {
                ShowSuggestions();
                return;
            }

            HideSuggestions(EM.SuggestionCleanupOperation.ResetIndex | EM.SuggestionCleanupOperation.ClearSelection);
            return;
        }
        private void UpdateSelectedItemIfSelectionIsDone(Key? key = null)
        {
            if (IsSelectionProcessInProgress(key))
            {
                return;
            }

            UpdateSelectedItemsFromSuggestionDropdown();
        }
        private bool IsSelectionProcessCompleted() => !IsSelectionProcessInProgress();
        private bool IsSelectionProcessInProgress(Key? keyUp = null)
        {
            if (!keyUp.HasValue)
            {
                return Keyboard.Modifiers == ModifierKeys.Control
                        || Keyboard.Modifiers == ModifierKeys.Shift;
            }

            return keyUp != Key.LeftCtrl && keyUp != Key.RightCtrl;
        }
        /// <summary>
        /// Removes all invalid texts from RichTextBox except selected item
        /// </summary>
        private void RemoveInvalidTexts()
        {
            try
            {
                //Unsubscribe handlers first
                if (!UnsubscribeHandler())
                {
                    //Failed to unsubscribe, return
                    return;
                }

                RichTextBoxElement.RemoveRunBlocks();
            }
            finally
            {
                //Subscribe back
                SubsribeHandler();
            }
        }
        #endregion

        #region Handler subscribe/unsubscribe
        /// <summary>
        /// Subscribes to events for controls
        /// </summary>
        private void SubsribeHandler()
        {
            //Check handler registration
            if (isHandlerRegistered)
            {
                //if already registered, return
                return;
            }

            //acquire a lock
            lock (handlerLock)
            {
                //double check registration
                if (isHandlerRegistered)
                {
                    //race condition, return
                    return;
                }

                //set handler flag to true
                isHandlerRegistered = true;

                //subscribe
                RichTextBoxElement.TextChanged += RichTextBoxElement_TextChanged;
            }
        }
        /// <summary>
        /// Unsubscribes to events for controls
        /// </summary>
        private bool UnsubscribeHandler()
        {
            if (RichTextBoxElement == null)
            {
                return true;
            }

            //Check handler registration
            if (!isHandlerRegistered)
            {
                //If already unsubscribed, return
                return false;
            }

            //acquire a lock
            lock (handlerLock)
            {
                //double check registration
                if (!isHandlerRegistered)
                {
                    //race condition, return
                    return false;
                }

                //set handler registration flag
                isHandlerRegistered = false;

                //unsubscribe
                RichTextBoxElement.TextChanged -= RichTextBoxElement_TextChanged;

                return true;
            }
        }
        #endregion

        #region Selection and Index
        private void HandleKeyboardUpKeyPress()
        {
            if (!HasAnySuggestion())
            {
                return;
            }

            ShowSuggestions();

            //If multi-selection
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                SuggestionElement.SelectMultiplePreviousItem();
                return;
            }

            SuggestionElement.SelectPreviousItem();
        }
        private void HandleKeyboardDownKeyPress()
        {
            if (!HasAnySuggestion())
            {
                return;
            }

            ShowSuggestions();

            //If multi-selection
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                SuggestionElement.SelectMultipleNextItem();
                return;
            }

            //Increment selected item index in drop-down
            SuggestionElement.SelectNextItem();
        }
        /// <summary>
        /// Tries to set item from entered text in RichTextBox
        /// </summary>
        /// <param name="itemString">entered text</param>
        /// <param name="forceAdd">Allows creation of new item</param>
        private void UpdateSelectedItemsFromEnteredText(string itemString)
        {
            itemString = itemString.Trim(ItemSeparator, ' ');

            if (IsItemAlreadySelected(itemString))
            {
                return;
            }

            object itemToAdd = GetItemToAdd(itemString);
            //If item is not available
            if (itemToAdd == null)
            {
                return;
            }

            AddToSelectedItems(itemToAdd);
            RaiseSelectionChangedEvent(new ArrayList(0), new[] { itemToAdd });
        }
        private bool IsItemAlreadySelected(string itemString) => SelectedItems?.Cast<object>()?.HasAnyExactMatch(itemString, LookUpContract, this) == true;
        private object GetItemToAdd(string itemString)
        {
            IEnumerable<object> controlItemSource = ItemSource?.Cast<object>();

            bool hasAnyMatch = controlItemSource.HasAnyExactMatch(itemString, LookUpContract, this);
            object itemToAdd = hasAnyMatch  //Check if any match
                                ? controlItemSource.GetExactMatch(itemString, LookUpContract, this) //Exact match is found
                                : (LookUpContract?.SupportsNewObjectCreation == true)   //Check if new object creation is supported by LookUpContract
                                    ? LookUpContract.CreateObject(this, itemString)     //Create new object using LookUpContract
                                    : null;                                             //cant create new item. return
            return itemToAdd;
        }
        private void AddToSelectedItems(object itemToAdd)
        {
            if (SelectedItems?.Contains(itemToAdd) == true)
            {
                return;
            }

            //Add item to Selected Item list
            SelectedItems?.Add(itemToAdd);
            //Add item in RichTextBox UI
            RichTextBoxElement.AddToParagraph(itemToAdd, CreateInlineUIElement);
        }
        private void AddSuggestionsToSelectedItems(IList itemsToAdd)
        {
            foreach (object item in itemsToAdd)
            {
                AddToSelectedItems(item);
            }

            RaiseSelectionChangedEvent(new ArrayList(0), new ArrayList(itemsToAdd));
        }
        /// <summary>
        /// Tries to set item from suggestion drop-down
        /// </summary>
        /// <param name="runTagToRemove"></param>
        /// <param name="itemObject"></param>
        private void UpdateSelectedItemsFromSuggestionDropdown()
        {
            try
            {
                //Unsubscribe handlers first
                if (!UnsubscribeHandler())
                {
                    //Failed to unsubscribe, return
                    return;
                }

                //Check if drop down is open or has any item selected
                if (!PopupElement.IsOpen || SuggestionElement.SelectedItems.Count < 1)
                {
                    return;
                }

                //Remove any user entered text if any
                RichTextBoxElement.RemoveRunBlocks();

                AddSuggestionsToSelectedItems(SuggestionElement.SelectedItems);

                //Hide drop-down
                HideSuggestions(EM.SuggestionCleanupOperation.ResetIndex | EM.SuggestionCleanupOperation.ClearSelection | EM.SuggestionCleanupOperation.ResetItemSource);
            }
            finally
            {
                //Subscribe back
                SubsribeHandler();
            }

            RichTextBoxElement.TryFocus();
        }
        #endregion

        #region Suggestion related methods
        /// <summary>
        /// Shows suggestion drop-down
        /// </summary>
        private void ShowSuggestions() => PopupElement.Show(HasAnySuggestion, () => SuggestionElement.CleanOperation(EM.SuggestionCleanupOperation.ResetIndex, ItemSource));
        private void HideSuggestions(EM.SuggestionCleanupOperation cleanupOperation) => PopupElement.Hide(null, () => SuggestionElement.CleanOperation(cleanupOperation, ItemSource));
        private bool HasAnySuggestion() => SuggestionElement.Items.Count > 0;
        private bool UpdateSuggestions(string userEnteredText)
        {
            //Get Items to be shown in suggestion drop-down for current text
            IEnumerable<object> itemsToAdd = ItemSource?.Cast<object>().GetSuggestions(userEnteredText, LookUpContract, this);

            //Add suggestion items to suggestion drop-down
            SuggestionElement.ItemsSource = itemsToAdd;

            return itemsToAdd?.Any() == true;
        }
        #endregion

        #region UI Element creation
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
        #endregion
        #endregion
    }
}