using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WP_TT
{
    static class DatetimeExtensions
    {
        public static bool IsWeekend(this DateTime date)
        {
            return new[] { DayOfWeek.Sunday, DayOfWeek.Saturday }.Contains(date.DayOfWeek);
        }
    }
}
