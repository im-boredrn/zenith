using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vintagestory.API.Common;

namespace zenith.Core.Helper
{

    public class ObjectReader
    {
        public static void DumpObject(EntityPlayer player, object obj,int depth = 0, int maxDepth = 4)
        {
            if (obj == null)
            {
                Logger.Log(player, "null");
                return;
            }
            string indent = new string(' ', depth * 2);
            Type type = obj.GetType();

            Logger.Log(player,$"{indent}{type.Name}");



            if (depth >= maxDepth)
                return;

            foreach (var field in type.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance))
            {
                try
                {
                    object value = field.GetValue(obj);

                    Logger.Log(player, $"{indent}  {field.Name} = {FormatValue(value)}" );

                    if (ShouldRecurse(value))
                    {
                        DumpObject(player,value, depth + 2, maxDepth);
                    }
                }
                catch
                {
                    Logger.Log(player, $"{indent}  {field.Name} = <unreadable>");
                }
            }
        

            foreach (PropertyInfo property in type.GetProperties(
         BindingFlags.Public |
         BindingFlags.NonPublic |
         BindingFlags.Instance))
            {
                try
                {
                    object value = property.GetValue(obj);

                    Logger.Log(player,$"{indent}  {property.Name} = {FormatValue(value)}");

                    if (ShouldRecurse(value))
                    {
                        DumpObject(player,value, depth + 2, maxDepth);
                    }
                }
                catch
                {
                    Logger.Log(player,$"{indent}  {property.Name} = <unreadable>");
                }
            }


        }

        private static bool ShouldRecurse(object value)
        {
            if (value == null)
                return false;

            Type type = value.GetType();

            return !type.IsPrimitive
                && type != typeof(string)
                && !type.IsEnum
                && !typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static string FormatValue(object value)
        {
            if (value == null)
                return "null";

            if (value is IEnumerable enumerable && value is not string)
                return $"[{value.GetType().Name}]";

            return value.ToString();
        }
    }
}
