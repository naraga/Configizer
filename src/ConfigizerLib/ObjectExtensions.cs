using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConfigizerLib
{
    public static class ObjectExtensions
    {
        const BindingFlags ParamsBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance |
                                  BindingFlags.NonPublic | BindingFlags.Public;

        public static object GetParamValue(this object o, string paramName, object defaultValue = null)
        {
            object value;

            // is it property ?
            var prop = o.GetType().GetProperty(paramName, ParamsBindingFlags);
            if (prop != null)
                value = prop.GetValue(o);
            else
            {
                // is it field ?
                var field = o.GetType().GetField(paramName, ParamsBindingFlags);
                if (field != null)
                {
                    value = field.GetValue(o);
                }
                else
                {
                    // not found, do we have default value?
                    if (defaultValue != null)
                        value = defaultValue;
                    else
                        throw new KeyNotFoundException(paramName);
                }
            }
            
            // if value we got is string let's interpolate it
            var s = value as string;
            if (s != null)
                return o.Interpolate(s, defaultValue as string);

            return value;
        }

        public static string Interpolate(this object source, string s, string defaultValue = null)
        {
            return Regex.Replace(s, @"\$\{(.+?)\}", delegate(Match match)
            {
                var paramExpr = match.Groups[1].Value;
                var paramNameAndFormatArr = paramExpr.Split(':');
                
                var paramName = paramNameAndFormatArr[0];
                var paramFormat = string.Join(":", paramNameAndFormatArr.Skip(1));
                var paramValue = source.GetParamValue(paramName, defaultValue);

                if (paramFormat == string.Empty)
                    return paramValue.ToString();

                var format = "{0:" + paramFormat + "}";
                return string.Format(format, paramValue);
            });
        }}
}