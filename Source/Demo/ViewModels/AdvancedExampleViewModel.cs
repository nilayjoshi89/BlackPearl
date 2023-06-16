using BlackPearl.Controls.Contract;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BlackPearl.Controls.Demo.ViewModels
{
    public class AdvancedExampleViewModel : BindableBase
    {
        public AdvancedExampleViewModel()
        {
			Source = new List<Person>(PersonDataProvider.GetDummyData());
			AdvanceLookUpContract = new AdvanceLookUpContract();
			SelectedItems = new ObservableCollection<Person>() { Source.FirstOrDefault() };
		}
		public List<Person> Source { get; set; }
		public ILookUpContract AdvanceLookUpContract { get; }
		public ObservableCollection<Person> SelectedItems { get; set; }
	}
}