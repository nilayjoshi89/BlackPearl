using System.Reflection;

namespace BlackPearl.Controls.Extension
{
    public static class ReflectionExtension
    {
        public static object GetPropertyValue(this object obj, string path)
        {
            if (string.IsNullOrEmpty(path) || obj == null)
            {
                return obj;
            }

            int dotIndex = path.IndexOf('.');
            if (dotIndex < 0)
            {
                return GetValue(obj, path);
            }

            obj = GetValue(obj, path.Substring(0, dotIndex + 1));
            path = path.Remove(0, dotIndex);

            return obj.GetPropertyValue(path);
        }

        private static object GetValue(object obj, string propertyName)
        {
            PropertyInfo propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo == null)
            {
                return null;
            }

            return propInfo.GetValue(obj);
        }
    }
}
