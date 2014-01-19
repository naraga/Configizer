using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConfigizerLib
{
    public static class ObjectExtensions
    {
        public static object GetParamValue(this object o, string paramName)
        {
            object value;

            var prop = o.GetType().GetProperty(paramName,
                BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop != null)
                value = prop.GetValue(o);
            else
            {
                var field = o.GetType().GetField(paramName,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null)
                    value = field.GetValue(o);
                else
                    throw new KeyNotFoundException(paramName);
            }

            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (value is string)
                return o.Interpolate((string)value);

            return value;
        }

        public static IEnumerable<string> GetAllParams(this object o)
        {
            return
                o.GetType().GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic |
                                          BindingFlags.Public).Select(p => p.Name)
                    .Union(
                        o.GetType()
                            .GetFields(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic |
                                       BindingFlags.Public).Select(p => p.Name));
        }

        public static string Interpolate(this object source, string s)
        {
            return Regex.Replace(s, @"\$\{(.+?)\}", delegate(Match match)
            {
                var paramExpr = match.Groups[1].Value;
                var paramNameAndFormat = paramExpr.Split(':');
                var paramValue = source.GetParamValue(paramNameAndFormat[0]);
                
                if (paramNameAndFormat.Length == 1)
                    return paramValue.ToString();

                var format = "{0:" + string.Join(":", paramNameAndFormat.Skip(1)) + "}";
                return string.Format(format, paramValue);
            });
        }}
}