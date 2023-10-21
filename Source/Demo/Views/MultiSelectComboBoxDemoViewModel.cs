using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

using BlackPearl.Controls.Contract;
using BlackPearl.Controls.CoreLibrary;
using BlackPearl.PrismUI;

using Prism.Commands;

namespace BlackPearl.Controls.Demo
{
    public class MultiSelectComboBoxDemoViewModel : BlackPearlViewModel
    {
        private readonly IDispatcherService dispatcherService;
        private char itemSeparator = ';';
        private char[] additionalItemSeparators = new char[0];
        private PersonDisplayPath selectedDisplayPath = PersonDisplayPath.Name;
        private ObservableCollection<Person> source = new ObservableCollection<Person>();
        private ObservableCollection<Person> selectedItems = new ObservableCollection<Person>();
        private bool isActive = true;
        private ILookUpContract lookupContract = new DefaultLookUpContract();
        private bool includeDiacriticItems = false;

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
        public bool IncludeDiacriticItems
        {
            get => includeDiacriticItems;
            set
            {
                includeDiacriticItems = value;
                RaisePropertyChanged(nameof(IncludeDiacriticItems));
                ForceReloadControl(changeInItemSource: true);
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
        public string SelectedItemsString => string.Join(ItemSeparator.ToString(), SelectedItems.Select(p => p.Name));
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

        public bool IsDefaultContract => LookupContract is DefaultLookUpContract;
        public bool IsDiacriticContract => LookupContract is DiacriticLookUpContract;
        public bool IsCustomContract => LookupContract is AdvanceLookUpContract;
        public ILookUpContract LookupContract
        {
            get => lookupContract;
            set
            {
                lookupContract = value;
                RaisePropertyChanged(nameof(LookupContract));
                RaisePropertyChanged(nameof(IsDefaultContract));
                RaisePropertyChanged(nameof(IsDiacriticContract));
                RaisePropertyChanged(nameof(IsCustomContract));
            }
        }
        public DelegateCommand<string> ChangeLookupContractCommand => new DelegateCommand<string>(ChangeLookupContract);

        private async void ForceReloadControl(bool changeInItemSource = false)
        {
            try
            {
                IsActive = false;

                List<Person> selection = null;
                if (changeInItemSource)
                {
                    await SetPersonItemSource();
                }
                else
                {
                    selection = SelectedItems.ToList();
                }

                await SetPersonSelectedItemRandom(selection);
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
            var data = await Task.Run(() => PersonDataProvider.GetDummyData());

            if (!IncludeDiacriticItems)
            {
                data = data.Skip(20);
            }

            await dispatcherService.Execute(() =>
            {
                Source = new ObservableCollection<Person>(data);
            });
        }
        private void ChangeLookupContract(string index)
        {
            if (index == null || index == "0")
            {
                LookupContract = new DefaultLookUpContract();
            }
            else if (index == "1")
            {
                LookupContract = new DiacriticLookUpContract();
            }
            else
            {
                LookupContract = new AdvanceLookUpContract();
            }

            ForceReloadControl();
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