using BlackPearl.Controls.Contract;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BlackPearl.Controls.Demo.ViewModels
{
    public class SimpleExampleViewModel : BindableBase
    {
        public SimpleExampleViewModel()
        {
			Source = new List<Person>(PersonDataProvider.GetDummyData());
			SimpleLookUpContract = new SimpleLookUpContract();
			SelectedItemsBasic = new ObservableCollection<Person>() { Source.FirstOrDefault() };
			SelectedItemsWithAdditionalItemSeparator = new ObservableCollection<Person>();
		}
		public List<Person> Source { get; set; }
		public ILookUpContract SimpleLookUpContract { get; }
		public ObservableCollection<Person> SelectedItemsBasic { get; set; }
		public ObservableCollection<Person> SelectedItemsWithAdditionalItemSeparator { get; set; }
	}
}