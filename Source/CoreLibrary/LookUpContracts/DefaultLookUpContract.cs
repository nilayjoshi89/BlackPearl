using System;
using System.Globalization;
using System.Linq;
using System.Text;
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
            return StringEqualsPredicate(RemoveDiacritics(itemString), RemoveDiacritics(seachString));
        }

        public bool IsItemMatchingSearchString(object sender, object item, string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return true;
            }

            string itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
                                    ?.ToString();
            return StringStartsWithPredicate(RemoveDiacritics(itemString), RemoveDiacritics(searchString));
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

        static string RemoveDiacritics(string text)
        {
            //"héllo" becomes "he<acute>llo", which in turn becomes "hello".
            return string.Concat(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
                .Normalize(NormalizationForm.FormC);
        }
    }
}
