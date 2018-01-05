using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    class TimeUnitStringConverter
    {
        static char HOURS_CHAR = 'h';
        static char DAYS_CHAR = 'd';

        public static double ConvertTime(string timeStr)
        {
            string toConvert = timeStr.Trim().Remove(timeStr.Length - 1);
            //Throwing exception withot replacing
            toConvert = toConvert.Replace(".", ",");
            char type = timeStr.Trim()[timeStr.Length - 1];
            double result = 0;
            result = Convert.ToDouble(toConvert);
            return result;
        }
    }
}
