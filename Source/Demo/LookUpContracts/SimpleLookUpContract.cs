using BlackPearl.Controls.Contract;
using BlackPearl.Controls.CoreLibrary;
using BlackPearl.Controls.Extension;
using System;

namespace BlackPearl.Controls.Demo
{
    public class SimpleLookUpContract : ILookUpContract
    {
        public bool SupportsNewObjectCreation => true;

        public object CreateObject(object sender, string searchString)
        {
            return new Person()
            {
                Name = searchString
            };
        }

        public bool IsItemEqualToString(object sender, object item, string seachString)
        {
            string itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
                                    ?.ToString();
            return StringEqualsPredicate(itemString, seachString);
        }

        public bool IsItemMatchingSearchString(object sender, object item, string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return true;
            }

            string itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
                                    ?.ToString();
            return StringStartsWithPredicate(itemString, searchString);
        }

        private static bool StringStartsWithPredicate(string value, string searchString)
        {
            return value != null
                && searchString != null
                && value.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase);
        }
        private static bool StringEqualsPredicate(string value1, string value2)
        {
            return value1 != null
               && value2 != null
               && string.Compare(value1, value2, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
