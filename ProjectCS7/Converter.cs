using System;
using System.Globalization;

namespace ProjectCS7
{
    static class Converter
    {
        public static object MakeType<T>(dynamic dt, IFormatProvider CurrentCulture)
        {
            object result = null;
            switch (dt.Value)
            {
                case string strFloat when float.TryParse(strFloat, NumberStyles.Any, CurrentCulture, out float floatVal):
                    {
                        result = floatVal;
                        break;
                    }
                case string strDouble when double.TryParse(strDouble, NumberStyles.Any, CurrentCulture, out double doubleVal):
                    {
                        result = (float)doubleVal;
                        break;
                    }
                case double dVal:
                    {
                        result = (float)dVal;
                        break;
                    }
                case string strDateTime when DateTime.TryParse(strDateTime, CurrentCulture, DateTimeStyles.AdjustToUniversal, out DateTime dtVal):
                    {
                        result = dtVal;
                        break;
                    }
                case DateTime dtValue:
                    {
                        result = dtValue;
                        break;
                    }
                default:
                    result = dt.Value;
                    break;
            }
            if (!(result is T))
            {
                string dynName = dt.Parent?.Name ?? "unknown";
                throw new FormatException($"Could not convert {dynName} containing '{result}' to Type {typeof(T)} !");
            }
            return result;
        }
    }
}

