using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

using BlackPearl.PrismUI;

using Prism.Commands;

namespace BlackPearl.Controls.Demo
{
    public class MultiSelectComboBoxDemoViewModel : BlackPearlViewModel
    {
        private readonly IDispatcherService dispatcherService;
        private char itemSeparator = ';';
        private char[] additionalItemSeparators = new char[0];
        private PersonDisplayPath selectedDisplayPath = PersonDisplayPath.City;
        private ObservableCollection<Person> source = new ObservableCollection<Person>();
        private ObservableCollection<Person> selectedItems = new ObservableCollection<Person>();
        private bool isActive = true;

        public MultiSelectComboBoxDemoViewModel(IDispatcherService dispatcherService)
        {
            this.dispatcherService = dispatcherService;
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }
        public DelegateCommand ForceRefreshCommand { get; }
        public ObservableCollection<Person> Source
        {
            get => source;
            set
            {
                source = value;
                RaisePropertyChanged(nameof(Source));
            }
        }

        public ObservableCollection<Person> SelectedItems
        {
            get => selectedItems;
            set
            {
                selectedItems = value;
                RaisePropertyChanged(nameof(SelectedItems));
                RaisePropertyChanged(nameof(SelectedItemsString));
            }
        }
        public string SelectedItemsString => string.Join(ItemSeparator, SelectedItems.Select(p => p.Name));
        public DelegateCommand<SelectionChangedEventArgs> SelectionChangedEventCommand => new DelegateCommand<SelectionChangedEventArgs>(SelectionChangedEventAction);

        public string DisplayMemberPath => SelectedDisplayPath.ToString();
        public PersonDisplayPath SelectedDisplayPath
        {
            get => selectedDisplayPath;
            set
            {
                selectedDisplayPath = value;
                RaisePropertyChanged(nameof(SelectedDisplayPath));
                RaisePropertyChanged(nameof(DisplayMemberPath));
                ForceReloadControl();
            }
        }

        public char[] ItemSeparatorSource => new char[] { ';', ',', '|', '~' };
        public char ItemSeparator
        {
            get => itemSeparator;
            set
            {
                itemSeparator = value;
                RaisePropertyChanged(nameof(ItemSeparator));
                ForceReloadControl();
            }
        }
        public char[] AdditionalItemSeparators
        {
            get => additionalItemSeparators;
            set
            {
                additionalItemSeparators = value;
                RaisePropertyChanged(nameof(AdditionalItemSeparators));
                ForceReloadControl();
            }
        }
        public DelegateCommand<CheckBox> AdditionalSeparatorCheckCommand => new DelegateCommand<CheckBox>(SetAdditionalSeparator);

        private async void ForceReloadControl()
        {
            try
            {
                IsActive = false;
                var currentSelection = SelectedItems.ToList();
                await SetPersonSelectedItemRandom(currentSelection);
            }
            catch { }
            finally
            {
                IsActive = true;
            }
        }

        public override bool KeepAlive => true;
        public override async Task OnLoad()
        {
            try
            {
                IsActive = false;
                await SetPersonItemSource();

                await SetPersonSelectedItemRandom();
            }
            catch { }
            finally
            {
                IsActive = true;
            }
        }
        private void SelectionChangedEventAction(SelectionChangedEventArgs eventArgs) => RaisePropertyChanged(nameof(SelectedItemsString));
        private void SetAdditionalSeparator(CheckBox checkBox)
        {
            if (checkBox == null)
                return;

            var separator = checkBox.Content.ToString()[0];
            if (checkBox.IsChecked == true)
            {
                var newValue = AdditionalItemSeparators.ToList();
                newValue.Add(separator);
                AdditionalItemSeparators = newValue.ToArray();
                return;
            }

            AdditionalItemSeparators = AdditionalItemSeparators.Where(i => i != separator).ToArray();
        }
        private async Task SetPersonSelectedItemRandom(System.Collections.Generic.List<Person> currentSelection = null)
        {
            if (currentSelection == null)
            //if (currentSelection?.Any() != true)
            {
                var item1 = Source[new Random().Next(0, Source.Count - 1)];
                currentSelection = new System.Collections.Generic.List<Person> { item1 };
            }

            await dispatcherService.Execute(() =>
            {
                var newSelection = new ObservableCollection<Person>(currentSelection);
                SelectedItems = newSelection;
            });
        }
        private async Task SetPersonItemSource()
        {
            var data = await Task.Run(() => new ObservableCollection<Person>(PersonDataProvider.GetDummyData()));
            await dispatcherService.Execute(() =>
            {
                Source = data;
            });
        }
    }

    public enum PersonDisplayPath
    {
        Name,
        Company,
        City,
        Zip,
        Info
    }
}