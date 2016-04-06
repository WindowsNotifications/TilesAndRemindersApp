using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TilesAndRemindersLibrary.Helpers
{
    public static class ArgumentsHelper
    {
        internal const string ActionKey = "Action";
        internal const string Arguments = "Arguments";

        private static readonly Type[] ArgumentsTypes;

        static ArgumentsHelper()
        {
            var assembly = typeof(ArgumentsHelper).GetTypeInfo().Assembly;

            ArgumentsTypes = assembly.GetTypes().Where(t => t.GetTypeInfo().IsSubclassOf(typeof(BaseArguments)) && t.GetConstructor(Type.EmptyTypes) != null).ToArray();
        }

        public static BaseArguments ParseArguments(string args)
        {
            QueryString queryString = QueryString.Parse(args);

            string action = queryString[ActionKey];

            string argumentsName = action + Arguments;

            Type type = ArgumentsTypes.FirstOrDefault(i => i.Name.Equals(argumentsName));

            if (type == null)
                return null;

            var answer = Activator.CreateInstance(type) as BaseArguments;

            foreach (var property in type.GetProperties().Where(i => i.CanWrite))
            {
                string value;

                if (queryString.TryGetValue(property.Name, out value))
                {
                    object convertedValue;

                    if (TryConvertFromString(value, property.PropertyType, out convertedValue))
                        property.SetValue(answer, convertedValue);
                }
            }

            return answer;
        }

        private static bool TryConvertFromString(string input, Type targetType, out object obj)
        {
            // If WinRT supported TypeDescriptor, this would be automatic...

            if (targetType == typeof(string))
            {
                obj = input;
                return true;
            }
            
            if (targetType == typeof(int))
            {
                int integer;
                if (int.TryParse(input, out integer))
                {
                    obj = integer;
                    return true;
                }

                obj = null;
                return false;
            }

            obj = null;
            return false;
        }
    }

    public abstract class BaseArguments
    {
        public BaseArguments()
        {
            var name = this.GetType().Name;

            if (!name.EndsWith(ArgumentsHelper.Arguments))
                throw new Exception("Arguments class must end with the word 'Arguments'");

            Action = name.Substring(0, name.Length - ArgumentsHelper.Arguments.Length);
        }

        public readonly string Action;

        /// <summary>
        /// Serializes the arguments into a query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            QueryString queryString = new QueryString();

            TypeInfo typeInfo = this.GetType().GetTypeInfo();

            // Write the action
            queryString.Add(ArgumentsHelper.ActionKey, this.Action);
            
            // Add all remaining values
            foreach (var property in typeInfo.DeclaredProperties.Where(i => i.CanRead))
            {
                // Skip Action since we wrote it at the beginning
                if (property.Name.Equals("Action"))
                    continue;

                queryString.Add(property.Name, property.GetValue(this).ToString());
            }

            return queryString.ToString();
        }
    }

    public class MarkCompleteArguments : BaseArguments
    {
        public int TaskId { get; set; }
    }
}
