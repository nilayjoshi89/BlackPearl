using System;

namespace BlackPearl.Controls.Library
{
    public partial class MultiSelectCombobox
    {
        /// <summary>
        /// Look-up contract for custom search behavior
        /// </summary>
        public interface ILookUpContract
        {
            /// <summary>
            /// Whether contract supports creation of new object from user entered text
            /// </summary>
            bool SupportsNewObjectCreation { get; }
            /// <summary>
            /// Method to check if item matches searchString
            /// </summary>
            /// <param name="sender">control</param>
            /// <param name="item">item to check</param>
            /// <param name="searchString">search string</param>
            /// <returns>true/false</returns>
            bool IsItemMatchingSearchString(object sender, object item, string searchString);
            /// <summary>
            /// Checks if item matches searchString or not
            /// </summary>
            /// <param name="sender">control</param>
            /// <param name="item">item to check</param>
            /// <param name="seachString">search string</param>
            /// <returns>true if matches otherwise false</returns>
            bool IsItemEqualToString(object sender, object item, string seachString);
            /// <summary>
            /// Creates object from provided string
            /// This method need to be implemented only when SupportsNewObjectCreation is set to true
            /// </summary>
            /// <param name="sender">control</param>
            /// <param name="searchString">text from which object need to be created</param>
            /// <returns>newly created object</returns>
            object CreateObject(object sender, string searchString);
        }
        /// <summary>
        /// Default LookUpContract implementation
        /// </summary>
        public class DefaultLookUpContract : ILookUpContract
        {
            public bool SupportsNewObjectCreation => false;

            public object CreateObject(object sender, string searchString)
            {
                throw new NotImplementedException();
            }

            public bool IsItemEqualToString(object sender, object item, string seachString)
            {
                var itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
                                        ?.ToString();
                return StringEqualsPredicate(itemString, seachString);
            }

            public bool IsItemMatchingSearchString(object sender, object item, string searchString)
            {
                var itemString = item?.GetPropertyValue((sender as MultiSelectCombobox)?.DisplayMemberPath)
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
                    && (string.Compare(value1.ToString(), value2, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }
    }
}