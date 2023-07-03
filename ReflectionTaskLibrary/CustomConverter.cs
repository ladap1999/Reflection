using System;
using System.Linq;
using System.Reflection;

namespace ReflectionTaskLibrary
{
    public class CustomConverter
    {
        private string _resultString = "[section.begin]\r\n";

        public string Serialize(object model, string spaces = "")
        {
            if (IsTypeToConvert(model))
            {
                return model.ToString().Replace('.', ',');
            }
            Type type = model.GetType();
            PropertyInfo[] pi = type.GetProperties();

            foreach (var property in pi)
            {
                if (property.GetValue(model, null) != null)
                {

                    if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                    {
                        if (property.Name.ToLower() != "NotSerializableProperty".ToLower())
                        {
                            _resultString += spaces + GetPropName(property, model) + GetPropValue(property, model);
                        }
                    }
                    else
                    {
                        _resultString += spaces + GetPropName(property, model) + "\r\n";
                        _resultString += spaces + spaces.PadRight(10, ' ') + "[section.begin]\r\n";
                        Serialize(property.GetValue(model, null), spaces + spaces.PadRight(10, ' '));

                    }
                }
            }
            _resultString += spaces + "[section.end]\r\n";
            return _resultString.Trim();
        }

        public string GetPropName(PropertyInfo property, object model)
        {
            string customNameString = "";
            if (property.CustomAttributes.Any())
            {
                if (property.CustomAttributes.Single().ConstructorArguments.Any())
                {
                    customNameString = property
                        .CustomAttributes
                        .Single().ConstructorArguments
                        .First()
                        .ToString()
                        .Replace("\"", "") + " = ";
                }
                else
                {
                    customNameString = property.Name + " = ";
                }
            }
            return customNameString;
        }

        public string GetPropValue(PropertyInfo property, object model)
        {

            return property.GetValue(model, null).ToString().Replace('.', ',') + "\r\n";

        }

        public bool IsTypeToConvert(object value)
        {
            if (Equals(value, null))
            {
                return false;
            }

            Type objType = value.GetType();

            if (objType.IsPrimitive || value is decimal || value is DayOfWeek || value is string)
            {
                return true;
            }
            return false;
        }
    }
}
