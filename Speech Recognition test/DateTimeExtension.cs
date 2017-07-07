using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public static class DateTimeExtension
    {
        public static string ToMilliSecondString(this DateTime datetime) => datetime.ToString("yyyy-MM-dd HH.mm.ss.ffff");
    }
}
