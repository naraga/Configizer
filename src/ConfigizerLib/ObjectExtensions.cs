using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConfigizerLib
{
    public static class ObjectExtensions
    {
        public static object GetParamValue(this object o, string paramName, object defaultValue = null)
        {
            object value;

            const BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance |
                                              BindingFlags.NonPublic | BindingFlags.Public;
            var prop = o.GetType().GetProperty(paramName, bindingFlags);
            if (prop != null)
                value = prop.GetValue(o);
            else
            {
                var field = o.GetType().GetField(paramName, bindingFlags);
                value = field != null ? field.GetValue(o) : defaultValue;
            }

            var s = value as string;
            if (s != null)
                return o.Interpolate(s);

            return value;
        }

        public static string Interpolate(this object source, string s)
        {
            return Regex.Replace(s, @"\$\{(.+?)\}", delegate(Match match)
            {
                var paramExpr = match.Groups[1].Value;
                var paramNameAndFormatArr = paramExpr.Split(':');
                
                var paramName = paramNameAndFormatArr[0];
                var paramFormat = string.Join(":", paramNameAndFormatArr.Skip(1));
                var paramValue = source.GetParamValue(paramName);
                
                if(paramValue == null)
                    throw new KeyNotFoundException(paramName);

                if (paramFormat == string.Empty)
                    return paramValue.ToString();

                var format = "{0:" + paramFormat + "}";
                return string.Format(format, paramValue);
            });
        }}
}