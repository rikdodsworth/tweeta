using System.Windows;

namespace WP7HelperFX.Common
{
    public class DataTemplateHelper
    {
        public static object LoadFromDictionary(Application App, string template)
        {
            object item = App.Resources[template];

            return item;
        }
    }
}
