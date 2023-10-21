using System.Globalization;
using System.Linq;
using System.Text;

namespace BlackPearl.Controls.Extension
{
    public static class GeneralExtension
    {
        public static string RemoveDiacritics(this string text) =>
            //"héllo" becomes "he<acute>llo", which in turn becomes "hello".
            string.Concat(text.Normalize(NormalizationForm.FormD).Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)).Normalize(NormalizationForm.FormC);
    }
}