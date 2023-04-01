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
            ShowSelectedItemCommand = new DelegateCommand<IList<Person>>(ShowSelectedItemCommandAction);
            AdvanceLookUpContract = new AdvanceLookUpContract();
            SimpleLookUpContract = new SimpleLookUpContract();

            SelectedItems = new List<Person>() { Source.FirstOrDefault() };
            SelectedItems2 = new List<Person>();
            SelectedItems3 = new List<Person>();
            SelectedItems4 = new List<Person>();
            SelectedItems5 = new List<Person>();
        }
        public List<Person> Source { get; set; }
        public ICommand ShowSelectedItemCommand { get; set; }
        public ILookUpContract AdvanceLookUpContract { get; }
        public ILookUpContract SimpleLookUpContract { get; }
        public List<Person> SelectedItems { get; set; }
        public List<Person> SelectedItems2 { get; set; }
        public List<Person> SelectedItems3 { get; set; }
        public List<Person> SelectedItems4 { get; set; }
        public List<Person> SelectedItems5 { get; set; }
        private void ShowSelectedItemCommandAction(IList<Person> data)
        {
            var stringBuilder = new StringBuilder();
            if (data is null)
            {
                return;
            }
            foreach (Person p in data)
            {
                stringBuilder.AppendLine(p.Name);
            }

            MessageBox.Show(stringBuilder.ToString(), "Selected items");
        }
    }
}
