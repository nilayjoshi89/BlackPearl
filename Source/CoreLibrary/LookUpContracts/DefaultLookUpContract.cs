using System;
using BlackPearl.Controls.Contract;
using BlackPearl.Controls.Extension;

namespace BlackPearl.Controls.CoreLibrary
{
    /// <summary>
    /// Default LookUpContract implementation
    /// </summary>
    public class DefaultLookUpContract : ILookUpContract
    {
        public bool SupportsNewObjectCreation => false;

        public object CreateObject(object sender, string searchString) => throw new NotImplementedException();

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
                && string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
