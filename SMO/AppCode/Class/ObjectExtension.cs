using KellermanSoftware.CompareNetObjects;

using Newtonsoft.Json.Linq;

using SMO.SimpleHelpers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SMO
{
    public static class ObjectExtension
    {
        public static object CloneObject(this object objSource)
        {
            //Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            object objTarget = Activator.CreateInstance(typeSource);
            //Get all the properties of source object type
            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //Assign all source property to taget object 's properties
            foreach (PropertyInfo property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    //else
                    //{
                    //    object objPropertyValue = property.GetValue(objSource, null);
                    //    if (objPropertyValue == null)
                    //    {
                    //        property.SetValue(objTarget, null, null);
                    //    }
                    //    else
                    //    {
                    //        property.SetValue(objTarget, objPropertyValue.CloneObject(), null);
                    //    }
                    //}
                }
            }
            return objTarget;
        }

        /// <summary>
        /// Compare 2 object class and return dictionary 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objSource"></param>
        /// <param name="objCompare"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IDictionary<string, DiffType> CompareObject<T>(this T objSource, T objCompare, string prefix = "") where T : class
        {
            var resultDiff = new Dictionary<string, DiffType>();
            var compare = new CompareLogic();
            var comparisonResult = compare.Compare(objSource, objCompare);
            //This is the comparison class

            //var comparisonResult = jdp.Diff(JsonConvert.SerializeObject(objSource), JsonConvert.SerializeObject(objCompare));
            foreach (var result in comparisonResult.Differences)
            {
                if (string.IsNullOrEmpty(result.Object1Value))
                {
                    resultDiff.Add($"{prefix}{result.PropertyName}", DiffType.DELELTE);
                }
                else if (string.IsNullOrEmpty(result.Object2Value))
                {
                    resultDiff.Add($"{prefix}{result.PropertyName}", DiffType.ADD);
                }
                else
                {
                    var diffProperty = objSource.GetType().GetProperties()
                        .FirstOrDefault(x => x.Name.Equals(result.ParentPropertyName));
                    if (diffProperty == null)
                    {
                        continue;
                    }
                    // different in an enumerable
                    if (diffProperty.PropertyType
                        .GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        var lstSource = (IEnumerable)diffProperty.GetValue(objSource);
                        var lstCompare = (IEnumerable)diffProperty.GetValue(objCompare);
                        var diff = ObjectDiffPatch.GenerateDiff(
                            new JObject { [result.ParentPropertyName] = JArray.FromObject(lstSource) },
                            new JObject { [result.ParentPropertyName] = JArray.FromObject(lstCompare) });
                        var oldValueProperties = diff.OldValues[result.ParentPropertyName].ToObject<JObject>().Properties();
                        foreach (var obj in (diff.NewValues[result.ParentPropertyName] as JObject).Properties())
                        {
                            var oldValue = diff.OldValues[result.ParentPropertyName][$"{obj.Name}"];
                            if (!string.IsNullOrEmpty(oldValue.Value<string>()))
                            {
                                resultDiff.Add($"{prefix}{result.ParentPropertyName}_{obj.Name}", DiffType.MODIFIED);

                                oldValueProperties.FirstOrDefault(x => x.Name.Equals($"{obj.Name}"))?.Remove();
                            }
                            else
                            {
                                resultDiff.Add($"{prefix}{result.ParentPropertyName}_{obj.Name}", DiffType.ADD);
                            }
                        }
                        foreach (var item in oldValueProperties)
                        {
                            resultDiff.Add($"{prefix}{result.ParentPropertyName}_{item.Name}", DiffType.DELELTE);
                        }
                    }
                    else
                    {
                        resultDiff.Add(result.MessagePrefix, DiffType.MODIFIED);
                    }
                }
            }
            return resultDiff;
        }
    }

    public static class DecimalExtension
    {
        public static string ToStringVN(this decimal value)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                var result = "";
                result = value.ToString(Global.NumberFormat, cul);
                return result;
            }
            catch
            {
                return "";
            }
        }

        public static string ToStringVnWithoutDecimal(this decimal value, bool hasDecimal = false)
        {
            try
            {
                if (hasDecimal)
                {
                    return string.Format("{0:n2}", value);
                }
                else
                {
                    return string.Format("{0:n0}", value);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static double ToDouble(this decimal value)
        {
            var canParse = double.TryParse(value.ToString(), out double result);
            if (canParse)
            {
                return result;
            }
            else
            {
                return double.NaN;
            }
        }
    }

    public static class StringExtension
    {
        public static string RemoveZeroWidthSpaces(this string input)
        {
            if (String.IsNullOrEmpty(input)) return input;
            char ZeroWidthSpace = (char)8203;
            if (input.Contains(ZeroWidthSpace.ToString()))
                return input.Replace(ZeroWidthSpace.ToString(), "");
            return input;
        }
    }

    public static class IListExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
            {
                yield return cur;
            }
            yield return value;
        }
    }
}