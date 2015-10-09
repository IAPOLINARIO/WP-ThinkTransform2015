using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WP_TT.Models;
using WP_TT.Services;

namespace WP_TT.Common
{
    /**
     * Build structured data to be used by HubSectionHistoricalChecks
     */
    class HistoricalChecksArrayBuilder
    {
        public async Task<IEnumerable<dynamic>> build(PersonalInfo personalInfo)
        {
            var ttRepository = new TTRepository();
            var storedHistoricalChecks = await ttRepository.FindAllByUserNameAsync(personalInfo.login);

            int daysToCheck;
            if (DateTime.Now.Day > 20)
            {  // do dia 21 ao 20 do próximo mês
                daysToCheck = DateTime.Now.Day - 20;

            }
            else
            { // do dia 21 do mês passado ao dia 20 do mês atual
                var day20 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 20).AddMonths(-1);
                daysToCheck = (int)DateTime.Now.Subtract(day20).TotalDays;
            }

            var historicalChecks = new List<TTCheck>(storedHistoricalChecks);

            // Add empty checks to days without checks
            for (var i = 0; i < daysToCheck; i++)
            {
                var day = DateTime.Now.AddDays(-i);
                if (!day.IsWeekend() && !HasCheckInDay(storedHistoricalChecks, day))
                {
                    historicalChecks.Add(new TTCheck() { DateTime = day, UserName = String.Empty });
                }
            }

            var historicalChecksGroupedByYearMonthAndDay = GetHistoricalChecksGroupedByYearMonthAndDay(historicalChecks).ToArray();
            
            return historicalChecksGroupedByYearMonthAndDay;
        }

        private bool HasCheckInDay(IEnumerable<TTCheck> checks, DateTime day)
        {
            return checks.Any(s => 
                s.DateTime.Year == day.Year &&
                s.DateTime.Month == day.Month &&
                s.DateTime.Day == day.Day
            );
        }

        private IEnumerable<dynamic> GetHistoricalChecksGroupedByYearMonthAndDay(IEnumerable<TTCheck> checks)
        {
            return checks
                .OrderByDescending(s => s.DateTime)
                .GroupBy(y => y.DateTime.Month.ToString() + y.DateTime.Year.ToString())
                .Select(y => new
                {
                    month = y.First().DateTime,
                    checksGroupedByDay = GetChecksGroupedByDay(y)
                });
        }

        private IEnumerable<dynamic> GetChecksGroupedByDay(IEnumerable<dynamic> month)
        {
            return month.GroupBy(s => s.DateTime.Day)
                .Select(d => new
                {
                    day = d.First().DateTime,
                    color = getColorOfMonth(d),
                    collection = GetChecksCollection(d)
                });
        }

        private IEnumerable<dynamic> GetChecksCollection(IEnumerable<dynamic> day)
        {
            return (IsEmptyDayCheck(day)) ? null : day.Select(h => new
                {
                    hour = h.DateTime,
                    color = getColorOfMonth(day)
                });
        }

        private string getColorOfMonth(IEnumerable<dynamic> month)
        {
            return (IsEmptyDayCheck(month)) ? "#FC183C" : (month.Count() % 2 == 0 ? "#3FCED6" : "#FFD05F");
        }

        private bool IsEmptyDayCheck(IEnumerable<dynamic> day)
        {
            return (day.First().UserName == String.Empty);
        }
    }
}
