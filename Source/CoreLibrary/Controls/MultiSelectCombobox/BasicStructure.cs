using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using BlackPearl.Controls.Contract;

namespace BlackPearl.Controls.CoreLibrary
{
    [TemplatePart(Name = "rtxt", Type = typeof(RichTextBox))]
    [TemplatePart(Name = "popup", Type = typeof(Popup))]
    [TemplatePart(Name = "lstSuggestion", Type = typeof(ListBox))]
    public sealed partial class MultiSelectCombobox : Control, INotifyPropertyChanged
    {
        #region Constructor
        static MultiSelectCombobox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectCombobox), new FrameworkPropertyMetadata(typeof(MultiSelectCombobox)));
        }
        public MultiSelectCombobox()
        {
            PreviewKeyDown += MultiSelectCombobox_PreviewKeyDown;
            LostFocus += MultiSelectCombobox_LostFocus;
        }
        #endregion

        #region Template Parts
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RichTextBoxElement = GetTemplateChild("rtxt") as RichTextBox;
            PopupElement = GetTemplateChild("popup") as Popup;
            SuggestionElement = GetTemplateChild("lstSuggestion") as ListBox;
        }

        private RichTextBox richTextBoxElement;
        private RichTextBox RichTextBoxElement
        {
            get => richTextBoxElement;
            set
            {
                if (richTextBoxElement != null)
                {
                    richTextBoxElement.TextChanged -= RichTextBoxElement_TextChanged;
                }

                richTextBoxElement = value;

                if (richTextBoxElement != null)
                {
                    richTextBoxElement.SetParagraphAsFirstBlock();

                    //Add all selected items
                    foreach (object item in SelectedItems)
                    {
                        richTextBoxElement.AddToParagraph(item, CreateInlineUIElement);
                    }

                    richTextBoxElement.TextChanged += RichTextBoxElement_TextChanged;
                }
            }
        }

        private Popup PopupElement { get; set; }

        private ListBox suggestionElement;
        private ListBox SuggestionElement
        {
            get => suggestionElement;
            set
            {
                if (suggestionElement != null)
                {
                    suggestionElement.PreviewMouseUp -= SuggestionDropdown_PreviewMouseUp;
                    suggestionElement.PreviewKeyUp -= SuggestionElement_PreviewKeyUp;
                    suggestionElement.PreviewMouseDown -= SuggestionDropdown_PreviewMouseDown;
                }

                suggestionElement = value;
                suggestionElement.DisplayMemberPath = DisplayMemberPath;
                suggestionElement.ItemsSource = ItemSource;

                if (suggestionElement != null)
                {
                    suggestionElement.PreviewMouseUp += SuggestionDropdown_PreviewMouseUp;
                    suggestionElement.PreviewKeyUp += SuggestionElement_PreviewKeyUp;
                    suggestionElement.PreviewMouseDown += SuggestionDropdown_PreviewMouseDown;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Item source
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable), typeof(MultiSelectCombobox), new PropertyMetadata(ItemSourcePropertyChanged));
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
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(MultiSelectCombobox), new PropertyMetadata(DisplayMemberPathChanged));
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
        #endregion  

        #region Property changed callback
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
            multiChoiceControl.RichTextBoxElement?.ClearParagraph();

            //Add all selected items
            foreach (object item in selectedItems)
            {
                multiChoiceControl?.RichTextBoxElement?.AddToParagraph(item, multiChoiceControl.CreateInlineUIElement);
            }

            //Notify UI of these changes
            //multiChoiceControl.NotifyPropertyChanged(string.Empty);
        }
        /// <summary>
        /// When ItemSource property is changed
        /// </summary>
        /// <param name="d">control</param>
        /// <param name="e">arguments</param>
        private static void ItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSelectCombobox multiChoiceControl))
            {
                return;
            }

            if (multiChoiceControl.SuggestionElement == null)
            {
                return;
            }

            multiChoiceControl.SuggestionElement.ItemsSource = (e.NewValue as IEnumerable)?.Cast<object>();
        }
        /// <summary>
        /// Display member path change handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSelectCombobox msc)
                || msc.SuggestionElement == null)
            {
                return;
            }

            msc.SuggestionElement.DisplayMemberPath = e.NewValue?.ToString();
        }
        #endregion

        #region Routed Event
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            name: nameof(SelectionChanged),
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(SelectionChangedEventHandler), ownerType:
            typeof(MultiSelectCombobox));
        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// Raise SelectionChanged event
        /// </summary>
        /// <param name="removedItems">removed items</param>
        /// <param name="addedItems">added items</param>
        private void RaiseSelectionChangedEvent(IList removedItems, IList addedItems) => RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, removedItems, addedItems));
        #endregion

        #region INotifyPropertyChanged implementation
        private void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}