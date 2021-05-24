namespace BlackPearl.Controls.Library
{
    public static class ReflectionHelper
    {
        public static object GetPropertyValue(this object obj, string path)
        {
            if(string.IsNullOrEmpty(path) || obj == null)
            {
                return obj;
            }

            var dotIndex = path.IndexOf('.');
            if (dotIndex < 0)
            {
                return GetValue(obj, path);
            }

            obj = GetValue(obj, path.Substring(0, dotIndex + 1));
            path = path.Remove(0, dotIndex);

            return GetPropertyValue(obj, path);
        }

        private static object GetValue(object obj, string propertyName)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo == null)
                return null;

            return propInfo.GetValue(obj);
        }
    }
}
