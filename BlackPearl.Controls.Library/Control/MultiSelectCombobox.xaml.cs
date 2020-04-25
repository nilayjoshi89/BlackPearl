using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace BlackPearl.Controls.Library
{
    /// <summary>
    /// Interaction logic for MultiSelectCombobox.xaml
    /// </summary>
    public partial class MultiSelectCombobox : UserControl, IDisposable, INotifyPropertyChanged
    {
        #region Members
        private bool areHandlersRegistered = false;
        private readonly object handlerLock = new object();
        private Paragraph paragraph = null;
        private int selectionStart = -1, suggestionIndex = -1;
        #endregion

        #region Constructor
        public MultiSelectCombobox()
        {
            InitializeComponent();
            InitializeControl();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Item source
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable), typeof(MultiSelectCombobox));
        public IEnumerable ItemSource
        {
            get => (IEnumerable)GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        /// <summary>
        /// List of selected items
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(MultiSelectCombobox), new PropertyMetadata(SelectedItemsChanged));
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Char value that separates two selected items. Default value is ';'
        /// </summary>
        public static readonly DependencyProperty ItemSeparatorProperty =
            DependencyProperty.Register(nameof(ItemSeparator), typeof(char), typeof(MultiSelectCombobox), new PropertyMetadata(';'));
        public char ItemSeparator
        {
            get => (char)GetValue(ItemSeparatorProperty);
            set => SetValue(ItemSeparatorProperty, value);
        }

        /// <summary>
        /// Display member path - for complex object, we can set this to show value on given path
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(MultiSelectCombobox), new PropertyMetadata(string.Empty));
        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        /// <summary>
        /// ILookUpContract - implementation for custom behavior of Look-up and create. 
        /// If not set, default behavior will be set.
        /// </summary>
        public static readonly DependencyProperty LookUpContractProperty =
            DependencyProperty.Register(nameof(LookUpContract), typeof(ILookUpContract), typeof(MultiSelectCombobox), new PropertyMetadata(new DefaultLookUpContract()));
        public ILookUpContract LookUpContract
        {
            get => (ILookUpContract)GetValue(LookUpContractProperty);
            set => SetValue(LookUpContractProperty, value);
        }

        /// <summary>
        /// Style for selected items
        /// </summary>
        public static readonly DependencyProperty SelectedItemTextBlockStyleProperty =
            DependencyProperty.Register(nameof(SelectedItemTextBlockStyle), typeof(Style), typeof(MultiSelectCombobox), new PropertyMetadata(GetDefaultSelectedItemTextBlockStyle()));
        public Style SelectedItemTextBlockStyle
        {
            get => GetValue(SelectedItemTextBlockStyleProperty) as Style;
            set => SetValue(SelectedItemTextBlockStyleProperty, value);
        }

        /// <summary>
        /// Internal property - Show drop-down or not
        /// </summary>
        private static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(MultiSelectCombobox));
        private bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Internal property - Subset of Item source used as source for suggestions in drop down
        /// </summary>
        private static readonly DependencyProperty SuggestionItemsSourceProperty =
            DependencyProperty.Register(nameof(SuggestionItemsSource), typeof(ObservableCollection<object>), typeof(MultiSelectCombobox), new PropertyMetadata(new ObservableCollection<object>()));
        private ObservableCollection<object> SuggestionItemsSource
        {
            get => (ObservableCollection<object>)GetValue(SuggestionItemsSourceProperty);
            set => SetValue(SuggestionItemsSourceProperty, value);
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Event handler for RichTextBox text change event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">arg</param>
        private void Rtxt_TextChanged(object sender, TextChangedEventArgs e)
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
                if (rtxt.CaretPosition.Parent is Run runTag)
                {
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
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }
        }
        /// <summary>
        /// Key down event for grid control
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">arg</param>
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            //User can remove paragraph reference by 'Select all & delete' in RichTextBox
            //Following method call with make sure local paragraph remains part of RichTextBox
            ReInitializeParagraphIfRequired();

            switch (e.Key)
            {
                case Key.Down:
                    {
                        //If multi-selection
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            DoDownwardMultiSelection();
                            e.Handled = true;
                            break;
                        }
                        
                        //Reset multi-select flag
                        selectionStart = -1;
                        
                        //Open suggestion drop down for entered
                        //Or Increment selection index in drop-down

                        //Get current text
                        string currentText = GetUserEnteredText();

                        //Check if any suggestion available for given text
                        if (!HasAnyItemStartingWith(SuggestionItemsSource, currentText))
                        {
                            //No suggestion to show, return
                            return;
                        }

                        //Show drop-down
                        ShowSuggestionDropDown();
                        //Increment selected item index in drop-down
                        IncrementSelectedIndex();
                    }
                    break;
                case Key.Up:
                    {
                        //If multi-select
                        if(Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            DoUpwardMultiSelection();
                            e.Handled = true;
                            break;
                        }

                        //Reset multi-select flag
                        selectionStart = -1;

                        //Get current text
                        string currentText = GetUserEnteredText();
                        //Check if any suggestion available for given text
                        if (!HasAnyItemStartingWith(SuggestionItemsSource, currentText))
                        {
                            //No suggestion to show, return
                            return;
                        }
                        //Show drop-down
                        ShowSuggestionDropDown();
                        //Decrement selected item index in drop-down
                        DecrementSelectedIndex();
                    }
                    break;
                case Key.Enter:
                    {
                        //Try select item based on selection in Suggestion drop-down
                        TrySetSelectedItemFromSuggestionDropDown();
                    }
                    break;
                case Key.Escape:
                    {
                        //Hide suggestion drop-down
                        HideSuggestionDropDown();
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Event to handle scenario where User removes selected item from UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded
                || !(sender is TextBlock tb))
            {
                return;
            }

            tb.Unloaded -= Tb_Unloaded;
            SelectedItems?.Remove(tb.Tag);
        }
        /// <summary>
        /// Control lost focus event handling
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">arg</param>
        private void MultiChoiceControl_LostFocus(object sender, RoutedEventArgs e)
        {
            //Remove all invalid texts from
            RemoveInvalidTexts();

            //Hide drop-down
            HideSuggestionDropDown();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes basic required controls
        /// </summary>
        private void InitializeControl()
        {
            paragraph = new Paragraph() { Style = new Style() };
            rtxt.Document.Blocks.Clear();
            rtxt.Document.Blocks.Add(paragraph);

            SubsribeHandlers();
        }
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
                rtxt.TextChanged += Rtxt_TextChanged;
                grd.PreviewKeyDown += Grid_KeyDown;
                this.LostFocus += MultiChoiceControl_LostFocus;
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
                rtxt.TextChanged -= Rtxt_TextChanged;
                grd.PreviewKeyDown -= Grid_KeyDown;
                this.LostFocus -= MultiChoiceControl_LostFocus;

                return true;
            }
        }
        /// <summary>
        /// Shows or Hides Suggestion drop-down based on current text
        /// </summary>
        private void ShowHideSuggestionDropDown()
        {
            //Clear all previous suggestions
            SuggestionItemsSource.Clear();
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
            var itemsToAdd = GetItemsStartingWith(ItemSource?.Cast<object>(), currentText);

            //Double check, if there are items to display
            if (itemsToAdd?.Any() != true)
            {
                //No suggestion found
                HideSuggestionDropDown();
                return;
            }

            //Add suggestion items to suggestion drop-down
            foreach (var i in itemsToAdd)
            {
                SuggestionItemsSource.Add(i);
            }

            //Show suggestion drop-down
            ShowSuggestionDropDown();
        }
        /// <summary>
        /// Shows suggestion drop-down
        /// </summary>
        private void ShowSuggestionDropDown()
        {
            //Opens drop-down if not already
            if (!IsDropDownOpen)
            {
                IsDropDownOpen = true;
            }
        }
        /// <summary>
        /// Hides suggestion drop-down
        /// </summary>
        private void HideSuggestionDropDown()
        {
            //Closes drop-down if not already
            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
                suggestionIndex = -1;
                selectionStart = -1;
            }
        }
        /// <summary>
        /// Re-initializes paragraph ref in RichTextBox
        /// </summary>
        private void ReInitializeParagraphIfRequired()
        {
            if (rtxt.Document.Blocks.FirstBlock != paragraph)
            {
                rtxt.Document.Blocks.Clear();
                rtxt.Document.Blocks.Add(paragraph);
            }
        }
        /// <summary>
        /// Gets user entered text in RichTextBox
        /// </summary>
        /// <returns></returns>
        private string GetUserEnteredText()
        {
            return (rtxt.CaretPosition.Parent as Run)?.Text;
        }
        /// <summary>
        /// Increments selection index for suggestion drop-down
        /// </summary>
        private void IncrementSelectedIndex()
        {
            var totalCount = SuggestionItemsSource.Count;
            suggestionIndex = (suggestionIndex + 1 >= totalCount)
                                ? totalCount - 1
                                : suggestionIndex + 1;

            //Clear any previous selection
            lstSuggestion.SelectedItems.Clear();
            lstSuggestion.SelectedItems.Add(SuggestionItemsSource[suggestionIndex]);
        }
        /// <summary>
        /// Decrement selection index for suggestion drop-down
        /// </summary>
        private void DecrementSelectedIndex()
        {
            suggestionIndex = suggestionIndex < 1
                ? 0
                : suggestionIndex - 1;

            //Clear any previous selection
            lstSuggestion.SelectedItems.Clear();
            lstSuggestion.SelectedItems.Add(SuggestionItemsSource[suggestionIndex]);
        }
        /// <summary>
        /// Downward selection for suggestion drop-down
        /// </summary>
        private void DoDownwardMultiSelection()
        {
            var oldIndex = suggestionIndex;
            var totalCount = SuggestionItemsSource.Count;
            suggestionIndex = (suggestionIndex + 1 >= totalCount)
                                ? totalCount - 1
                                : suggestionIndex + 1;

            //If its boundary, return
            if (oldIndex == suggestionIndex)
                return;

            //If its first time - Start of selection
            if (selectionStart == -1)
            {
                //set previous index as selection start
                selectionStart = oldIndex;
                //Add current item to selected items list
                lstSuggestion.SelectedItems.Add(SuggestionItemsSource[suggestionIndex]);
                return;
            }

            //If selection is shrinking then remove previous selected element
            if (selectionStart > oldIndex)
            {
                lstSuggestion.SelectedItems.Remove(SuggestionItemsSource[oldIndex]);
                return;
            }

            //Otherwise, selection is growing, add current element to selected items list
            lstSuggestion.SelectedItems.Add(SuggestionItemsSource[suggestionIndex]);
        }
        /// <summary>
        /// Upward selection for suggestion drop-down
        /// </summary>
        private void DoUpwardMultiSelection()
        {
            var oldIndex = suggestionIndex;
            suggestionIndex = suggestionIndex < 1
                ? 0
                : suggestionIndex - 1;

            //If its boundary, return
            if (oldIndex == suggestionIndex)
                return;

            //If its first time - Start of selection
            if (selectionStart == -1)
            {
                //set previous index as selection start
                selectionStart = oldIndex;
                //Add current item to selected items list
                lstSuggestion.SelectedItems.Add(SuggestionItemsSource[suggestionIndex]);
                return;
            }

            //If selection is shrinking then remove previous selected element
            if (selectionStart < oldIndex)
            {
                lstSuggestion.SelectedItems.Remove(SuggestionItemsSource[oldIndex]);
                return;
            }

            //Otherwise, selection is growing, add current element to selected items list
            lstSuggestion.SelectedItems.Add(SuggestionItemsSource[suggestionIndex]);
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
            var itemToAdd = GetItemFromSource(ItemSource?.Cast<object>(), itemString);

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
            }
        }
        /// <summary>
        /// Gets item from source for given text
        /// </summary>
        /// <param name="source">Item source to search</param>
        /// <param name="textToSearch">text to search</param>
        /// <returns>Item from source if any else null</returns>
        private object GetItemFromSource(IEnumerable<object> source, string textToSearch)
        {
            //Uses LookUpContract to get exact item from source
            return source?.FirstOrDefault(i => LookUpContract?.IsItemEqualToString(this, i, textToSearch) == true);
        }
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
            rtxt.CaretPosition = rtxt.CaretPosition.DocumentEnd;
        }
        /// <summary>
        /// Create RichTextBox document element for given object
        /// </summary>
        /// <param name="objectToDisplay"></param>
        /// <returns></returns>
        private InlineUIContainer CreateInlineUIElement(object objectToDisplay)
        {
            TextBlock tb = new TextBlock()
            {
                //Text based on Display member path
                Text = objectToDisplay.GetPropertyValue(DisplayMemberPath)?.ToString() + ItemSeparator,
                //Selected TextBlock style
                Style = SelectedItemTextBlockStyle,
                //Set object in Tag for easy access for future operations
                Tag = objectToDisplay
            };

            tb.Unloaded += Tb_Unloaded;
            return new InlineUIContainer(tb);
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
                if (!IsDropDownOpen || lstSuggestion.SelectedItems.Count < 1)
                {
                    return;
                }

                //Hide drop-down
                HideSuggestionDropDown();

                //Remove any user entered text if any
                if (rtxt.CaretPosition.Parent is Run runElementToRemove)
                {
                    paragraph.Inlines.Remove(runElementToRemove);
                }

                foreach (var itemObject in lstSuggestion.SelectedItems)
                {
                    //Check if item is already selected or not
                    if (SelectedItems?.Contains(itemObject) == true)
                    {
                        return;
                    }

                    //Add item to selected item list
                    SelectedItems?.Add(itemObject);
                    //Add item to UI
                    AddItemToUI(itemObject);
                }

                lstSuggestion.SelectedItems?.Clear();
            }
            finally
            {
                //Subscribe back
                SubsribeHandlers();
            }
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

                for(int i=0;i<runTags.Count; i++)
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
        /// <summary>
        /// Default style TextBlock used for selected items
        /// </summary>
        /// <returns>Style of TextBlock</returns>
        private static Style GetDefaultSelectedItemTextBlockStyle()
        {
            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.LightBlue));
            style.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.SemiBold));
            style.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(0, 0, 5, 0)));
            return style;
        }
        /// <summary>
        /// When selected item property is changed
        /// </summary>
        /// <param name="d">control</param>
        /// <param name="e">arg</param>
        private static void SelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSelectCombobox multiChoiceControl
                && e.NewValue is IList selectedItems && selectedItems != null))
            {
                return;
            }

            //Clear everything in RichTextBox
            multiChoiceControl.paragraph.Inlines.Clear();

            //Add all selected items
            foreach (var item in selectedItems)
            {
                multiChoiceControl.AddItemToUI(item);
            }

            //Notify UI of these changes
            multiChoiceControl.NotifyPropertyChanged(string.Empty);
        }
        #region LookUp Methods
        /// <summary>
        /// Gets all items from source matching search criteria
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="textToSearch">search text</param>
        /// <returns>All items fulfilling search criteria</returns>
        private IEnumerable<object> GetItemsStartingWith(IEnumerable<object> source, string textToSearch)
        {
            return source?.Where(i => LookUpContract?.IsItemMatchingSearchString(this, i, textToSearch) == true);
        }
        /// <summary>
        /// Checks for any source item which match search criteria
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="textToSearch">search text</param>
        /// <returns>true if any item found else false</returns>
        private bool HasAnyItemStartingWith(IEnumerable<object> source, string textToSearch)
        {
            return source?.Any(i => LookUpContract?.IsItemMatchingSearchString(this, i, textToSearch) == true) == true;
        }
        /// <summary>
        /// Checks source for item having equivalent value as textToSearch
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="textToSearch">search text</param>
        /// <returns></returns>
        private bool HasAnyItem(IEnumerable<object> source, string textToSearch)
        {
            return source?.Any(i => LookUpContract?.IsItemEqualToString(this, i, textToSearch) == true) == true;
        }
        #endregion
        
        #endregion

        #region INotifyPropertyChanged implementation
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region IDisposable pattern implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeHandlers();
            }
        }
        #endregion
    }
}