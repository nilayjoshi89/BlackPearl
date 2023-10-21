using System;

using BlackPearl.Controls.Contract;
using BlackPearl.Controls.Extension;

namespace BlackPearl.Controls.CoreLibrary
{
    public class DiacriticLookUpContract : ILookUpContract
    {
        public bool SupportsNewObjectCreation => false;

        public object CreateObject(object sender, string searchString) => throw new NotSupportedException();

        public bool IsItemEqualToString(object sender, object item, string seachString)
        {
            string itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
                                    ?.ToString();
            return StringEqualsPredicate(itemString?.RemoveDiacritics(), seachString?.RemoveDiacritics());
        }

        public bool IsItemMatchingSearchString(object sender, object item, string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return true;
            }

            string itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
                                    ?.ToString();
            return StringStartsWithPredicate(itemString?.RemoveDiacritics(), searchString?.RemoveDiacritics());
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
