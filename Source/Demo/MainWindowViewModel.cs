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
        public static ICommand ShowSelectedItemCommand { get; } = new DelegateCommand<IList<Person>>(ShowSelectedItemCommandAction);

        private static void ShowSelectedItemCommandAction(IList<Person> data)
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
