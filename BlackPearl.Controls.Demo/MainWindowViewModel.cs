using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using BlackPearl.Controls.Contract;

using Prism.Commands;
using Prism.Mvvm;

namespace BlackPearl.Controls.Demo
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            Source = new List<Person>(PersonDataProvider.GetDummyData());
            SelectedItems = new List<Person>() { Source.FirstOrDefault() };
            SelectedItems2 = new List<Person>();
            ShowSelectedItemCommand = new DelegateCommand<IList<Person>>(ShowSelectedItemCommandAction);
            AdvanceLookUpContract = new AdvanceLookUpContract();
        }
        public List<Person> Source { get; set; }
        public List<Person> SelectedItems { get; set; }
        public List<Person> SelectedItems2 { get; set; }
        public ICommand ShowSelectedItemCommand { get; set; }
        public ILookUpContract AdvanceLookUpContract { get; }
        private void ShowSelectedItemCommandAction(IList<Person> data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var p in data)
            {
                stringBuilder.AppendLine(p.Name);
            }

            MessageBox.Show(stringBuilder.ToString(), "Selected items");
        }
    }
}
