using System.Collections;
using System.Collections.Generic;
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
        #endregion

        #region Control Event Handlers
        /// <summary>
        /// Suggestion drop down - key board key up
        /// Forces control to update selected item based on selection in suggestion drop down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestionElement_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!IsSelectionProcessAboutToComplete(e.Key))
            {
                return;
            }

            UpdateSelectedItemsFromSuggestionDropdown();
        }
        private void SuggestionDropdown_PreviewMouseDown(object sender, MouseButtonEventArgs e) => SuggestionElement.ClearSelection(IsSelectionProcessCompleted);
        /// <summary>
        /// Suggestion drop down - mouse key up
        /// Forces control to update selected item based on selection in suggestion drop down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestionDropdown_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((SuggestionElement?.SelectedItems?.Count ?? 0) == 0
                || IsSelectionProcessInProgress())
            {
                return;
            }

            UpdateSelectedItemsFromSuggestionDropdown();
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
                HideSuggestions(SuggestionCleanupOperation.ResetIndex | SuggestionCleanupOperation.ClearSelection);
            }
            catch { }
        }
        private void MultiSelectCombobox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //User can remove paragraph reference by 'Select all & delete' in RichTextBox
                //Following method call with make sure local paragraph remains part of RichTextBox
                RichTextBoxElement.SetupOrCheckParagraph();

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
                            HideSuggestions(SuggestionCleanupOperation.ResetIndex | SuggestionCleanupOperation.ClearSelection);
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

                if (string.IsNullOrEmpty(userEnteredText)
                    || !userEnteredText.EndsWith(ItemSeparator.ToString()))
                {
                    bool hasAnySuggestionToShow = UpdateSuggestions(userEnteredText);
                    if (hasAnySuggestionToShow)
                    {
                        ShowSuggestions();
                        return;
                    }

                    HideSuggestions(SuggestionCleanupOperation.ResetIndex | SuggestionCleanupOperation.ClearSelection);
                    return;
                }

                //Unsubscribe handlers first
                if (!UnsubscribeHandlers())
                {
                    //Failed to unsubscribe, return
                    return;
                }

                //Hide suggestion drop-down
                //Reset suggestion drop down list
                HideSuggestions(SuggestionCleanupOperation.ResetIndex | SuggestionCleanupOperation.ResetItemSource);

                //User is expecting to complete item selection
                //Check if current text is blank + separator
                if (string.IsNullOrWhiteSpace(userEnteredText.Trim(ItemSeparator)))
                {
                    //there's nothing to select
                    //set current text to empty
                    RichTextBoxElement.ResetCurrentText();
                    return;
                }

                //User has entered valid text + separator
                RichTextBoxElement.RemoveRunBlocks();
                //Try select item from source based on current entered text
                //Set force-add to true as user is confident about selected text
                UpdateSelectedItemsFromEnteredText(userEnteredText);
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
                RichTextBoxElement.TextChanged += RichTextBoxElement_TextChanged;
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
                RichTextBoxElement.TextChanged -= RichTextBoxElement_TextChanged;

                return true;
            }
        }
        private bool UpdateSuggestions(string userEnteredText)
        {
            //Get Items to be shown in suggestion drop-down for current text
            IEnumerable<object> itemsToAdd = ItemSource?.Cast<object>().GetSuggestions(userEnteredText, LookUpContract, this);

            //Add suggestion items to suggestion drop-down
            SuggestionElement.ItemsSource = itemsToAdd;

            return itemsToAdd?.Any() == true;
        }
        /// <summary>
        /// Shows suggestion drop-down
        /// </summary>
        private void ShowSuggestions() => PopupElement.Show(HasAnySuggestion, ResetSelectedIndex);
        private void HideSuggestions(SuggestionCleanupOperation cleanupOperation) => PopupElement.Hide(null, () => PerformSuggestionCleanupOperation(cleanupOperation));
        private bool HasAnySuggestion() => SuggestionElement.GetItemSource()?.Any() == true;
        /// <summary>
        /// Tries to set item from entered text in RichTextBox
        /// </summary>
        /// <param name="itemString">entered text</param>
        /// <param name="forceAdd">Allows creation of new item</param>
        private void UpdateSelectedItemsFromEnteredText(string itemString)
        {
            itemString = itemString.Trim(ItemSeparator, ' ');

            //Default LookUpContract implementation does not support item creation

            //If LookUpContract does not supports creation of new item
            //And Entered text does not have any valid suggestion

            IEnumerable<object> controlItemSource = ItemSource?.Cast<object>();

            if ((LookUpContract?.SupportsNewObjectCreation != true)
                && !controlItemSource.HasAnyExactMatch(itemString, LookUpContract, this))
            {
                //Not a valid item, return
                return;
            }

            //item text is a valid in ItemSource or can be created using LookUpContract
            //Check if item is already selected or not
            if (SelectedItems?.Cast<object>().HasAnyExactMatch(itemString, LookUpContract, this) != false)
            {
                //already selected, return
                return;
            }

            //Try to get item from source based on itemString
            object itemToAdd = controlItemSource.GetExactMatch(itemString, LookUpContract, this);

            if (itemToAdd == null && LookUpContract?.SupportsNewObjectCreation == true)
            {
                //If item is not found in ItemSource look-up and LookUpContract supports creation of new item
                itemToAdd = LookUpContract.CreateObject(this, itemString);
            }

            //If item is not available
            if (itemToAdd == null)
            {
                return;
            }

            AddToSelectedItems(itemToAdd);
            RaiseSelectionChangedEvent(new ArrayList(0), new[] { itemToAdd });
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
        private void UpdateSelectedItemsFromSuggestionDropdown()
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

                //Remove any user entered text if any
                RichTextBoxElement.RemoveRunBlocks();

                AddSuggestionsToSelectedItems(SuggestionElement.SelectedItems);

                //Hide drop-down
                HideSuggestions(SuggestionCleanupOperation.ResetIndex | SuggestionCleanupOperation.ClearSelection | SuggestionCleanupOperation.ResetItemSource);
            }
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }

            RichTextBoxElement.TryFocus();
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

                RichTextBoxElement.RemoveRunBlocks();
            }
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }
        }
        private void HandleKeyboardUpKeyPress()
        {
            if (!HasAnySuggestionToShow())
            {
                return;
            }

            ShowSuggestions();

            //If multi-selection
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                //DoUpwardMultiSelection();
                SuggestionElement.SelectMultiplePreviousItem();
                return;
            }

            //Decrement selected item index in drop-down
            SuggestionElement.SelectPreviousItem();
        }
        private void HandleKeyboardDownKeyPress()
        {
            if (!HasAnySuggestionToShow())
            {
                return;
            }

            ShowSuggestions();

            //If multi-selection
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                //DoDownwardMultiSelection();
                SuggestionElement.SelectMultipleNextItem();
                return;
            }

            //Increment selected item index in drop-down
            SuggestionElement.SelectNextItem();
        }
        private bool HasAnySuggestionToShow() => SuggestionElement?.GetItemSource()?.Any() == true;
        private bool IsSelectionProcessCompleted() => !(Keyboard.IsKeyDown(Key.LeftCtrl)
                                                        || Keyboard.IsKeyDown(Key.RightCtrl)
                                                        || Keyboard.IsKeyDown(Key.LeftShift)
                                                        || Keyboard.IsKeyDown(Key.RightShift));
        private bool IsSelectionProcessAboutToComplete(Key key) => key == Key.LeftCtrl || key == Key.RightCtrl;
        public bool IsSelectionProcessInProgress() => Keyboard.Modifiers == ModifierKeys.Control
                                                        || Keyboard.Modifiers == ModifierKeys.Shift;
        private void PerformSuggestionCleanupOperation(SuggestionCleanupOperation operation)
        {
            if ((operation & SuggestionCleanupOperation.ClearSelection) == SuggestionCleanupOperation.ClearSelection)
            {
                SuggestionElement.ClearSelection();
            }

            if ((operation & SuggestionCleanupOperation.ResetIndex) == SuggestionCleanupOperation.ResetIndex)
            {
                ResetSelectedIndex();
            }

            if ((operation & SuggestionCleanupOperation.ResetItemSource) == SuggestionCleanupOperation.ResetItemSource)
            {
                ResetSuggestionItemSource();
            }
        }
        private void ResetSelectedIndex()
        {
            suggestionElement.SetSelectionStart(-1);
            suggestionElement.SetSelectionEnd(-1);
        }
        private void ResetSuggestionItemSource() => SuggestionElement.ItemsSource = ItemSource;
        #endregion

        internal enum SuggestionCleanupOperation
        {
            ResetIndex = 1,
            ClearSelection = 2,
            ResetItemSource = 4
        };
    }
}