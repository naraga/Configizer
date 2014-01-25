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

        public static void SetParamValue(this object o, string paramName, string value)
        {
            var prop = o.GetType().GetProperty(paramName, ParamsBindingFlags);
            if (prop != null)
                prop.SetValue(o, value);
            else
            {
                var field = o.GetType().GetField(paramName, ParamsBindingFlags);
                if (field != null)
                    field.SetValue(o, value);
                else
                    throw new KeyNotFoundException(paramName);
            }
        }

        public static object GetParamValue(this object o, string paramName, object defaultValue = null)
        {
            object value;

            var prop = o.GetType().GetProperty(paramName, ParamsBindingFlags);
            if (prop != null)
                value = prop.GetValue(o);
            else
            {
                var field = o.GetType().GetField(paramName, ParamsBindingFlags);
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