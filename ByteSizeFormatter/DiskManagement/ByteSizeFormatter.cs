using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DiskManagement
{
    public class ByteSizeFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if(formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {   
            // Get bytes decimal, double, float, int, long, short, uint, ulong, ushort
            ulong bytes;
            if (arg is decimal || arg is double || arg is float || arg is int || arg is long || arg is short || arg is uint || arg is ulong || arg is ushort)
            {
                bytes = Convert.ToUInt64(arg);
            }
            else
            {
                try
                {
                    return HandleOtherFormats(format, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                }
            }

            // Parse format for suffix and precision
            string prefix = String.Empty;
            int precision = -1;
            string pattern = @"([sb]f|[bkmgtp]i?)(\d*)";
            Match match = Regex.Match(format, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!String.IsNullOrEmpty(match.Value))
            {
                prefix = match.Groups[1].Value.ToString().ToLowerInvariant();
                if (!String.IsNullOrEmpty(match.Groups[2].Value))
                {
                    precision = Convert.ToInt32(match.Groups[2].Value);
                }
            }

            // Determine conversion parameters
            char[] prefixes = new char[] {'b', 'k', 'm', 'g', 't', 'p'};
            double divisor = 1D;
            string suffix = String.Empty;
            bool isBinary = false;
            double multiplier = 1000D;
            if (prefix.Contains('i') || prefix == "bf")
            {
                isBinary = true;
                multiplier = 1024D;
            }
            
            if (prefix == "bf" || prefix == "sf")
            {
                for (int i = 5; i >= 0; i--)
                {
                    divisor = Math.Pow(multiplier, (double)i);
                    if (i == 0)
                    {
                        divisor = 1D;
                        precision = 0;
                        suffix = "bytes";
                        break;
                    }
                    else if (bytes / (divisor) > 1UL)
                    {
                        if (precision < 0) precision = i;
                        suffix = isBinary ? prefixes[i].ToString().ToUpper() + "iB" : prefixes[i].ToString().ToUpper() + "B";
                        break;
                    }
                }
            }
            else
            {
                bool found = false;
                for(int i=0; i < prefixes.Count(); i++)
                {
                    if (prefix.Contains(prefixes[i]))
                    {
                        if (i == 0)
                        {
                            divisor = 1UL;
                            precision = 0;
                            suffix = "bytes";
                            found = true;
                            break;
                        }
                        else
                        {
                            divisor = Math.Pow(multiplier, (double)i);
                            suffix = isBinary ? prefixes[i].ToString().ToUpper() + "iB" : prefixes[i].ToString().ToUpper() + "B";
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    try
                    {
                        return HandleOtherFormats(format, arg);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                    }
                }
            }
            
            if (precision < 0) precision = 2;
            if (precision == 0)
                return String.Format("{0:N0} {1}", Convert.ToDecimal(bytes / divisor), suffix);
            else
                return String.Format("{0:N" + precision + "} {1}", Convert.ToDecimal(bytes / divisor), suffix);
        }

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else
                return String.Empty;
        }
    }
}
