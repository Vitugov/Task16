using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Task16.Other
{
    public class DictionaryToBindableListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = value as Dictionary<string, string>;
            if (dictionary == null) return null;

            // Преобразовываем словарь в список анонимных объектов с именем и email
            var bindableList = dictionary.Select(kvp => new { Name = kvp.Key, Email = kvp.Value }).ToList();
            return bindableList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
