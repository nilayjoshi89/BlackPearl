using System.Linq;

using BlackPearl.Controls.Contract;

namespace BlackPearl.Controls.Demo
{
    public class AdvanceLookUpContract : ILookUpContract
    {
        public bool SupportsNewObjectCreation => true;

        public object CreateObject(object sender, string searchString)
        {
            if (searchString?.Count(c => c == ',') != 2)
            {
                return null;
            }

            int firstIndex = searchString.IndexOf(',');
            int lastIndex = searchString.LastIndexOf(',');

            return new Person()
            {
                Name = searchString.Substring(0, firstIndex),
                Company = searchString.Substring(firstIndex + 1, lastIndex - firstIndex - 1),
                Zip = searchString.Length >= lastIndex ? searchString.Substring(lastIndex + 1) : string.Empty
            };
        }

        public bool IsItemEqualToString(object sender, object item, string seachString)
        {
            if (!(item is Person std))
            {
                return false;
            }

            return string.Compare(seachString, std.Name, System.StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public bool IsItemMatchingSearchString(object sender, object item, string searchString)
        {
            if (!(item is Person person))
            {
                return false;
            }

            if (string.IsNullOrEmpty(searchString))
            {
                return true;
            }

            return person.Name?.ToLower()?.Contains(searchString?.ToLower()) == true
                || person.Company.ToString().ToLower()?.Contains(searchString?.ToLower()) == true
                || person.Zip?.ToLower()?.Contains(searchString?.ToLower()) == true;
        }
    }
}
