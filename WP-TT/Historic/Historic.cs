using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WP_TT.Models;
using WP_TT.Services;

namespace WP_TT.Historic
{
    public static class Historic
    {
        private static ObservableCollection<Month> historic;

        public static async Task<ObservableCollection<Month>> Get()
        {
            if (historic != null)
                return historic;

            PersonalInfoRespository repository = new PersonalInfoRespository();

            PersonalInfo personalInfo = await repository.LoadAsync();

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

            historic = getMonths(historicalChecks);

            return historic;
        }

        private static bool HasCheckInDay(IEnumerable<TTCheck> checks, DateTime day)
        {
            return checks.Any(s => 
                s.DateTime.Year == day.Year &&
                s.DateTime.Month == day.Month &&
                s.DateTime.Day == day.Day
            );
        }

        private static ObservableCollection<Month> getMonths(IEnumerable<TTCheck> checks)
        {
            return new ObservableCollection<Month>(
                checks
                    .OrderByDescending(c => c.DateTime)
                    .GroupBy(c => c.DateTime.Month.ToString() + c.DateTime.Year.ToString())
                    .Select(g => new Month
                    {
                        Check = g.First(),
                        Days = getDays(g)
                    }));
        }

        private static ObservableCollection<Day> getDays(IEnumerable<TTCheck> checks)
        {
            return new ObservableCollection<Day>(
                checks
                    .GroupBy(c => c.DateTime.Day)
                    .Select(g => new Day
                    {
                        Check = g.First(),
                        Hours = string.IsNullOrEmpty(g.First().UserName) ?
                            new ObservableCollection<Hour>() :
                            getHours(g)
                    }));
        }

        private static ObservableCollection<Hour> getHours(IEnumerable<TTCheck> checks)
        {
            if (IsEmptyDayCheck(checks))
                return null;
            else
            {
                ObservableCollection<Hour> result = new ObservableCollection<Hour>();

                var hours = checks
                        .Select(c => new Hour(result)
                        {
                            Check = c
                        });

                foreach (Hour hour in hours)
                    result.Add(hour);

                return result;
            }                    
        }

        private static bool IsEmptyDayCheck(IEnumerable<TTCheck> checks)
        {
            return (checks.First().UserName == String.Empty);
        }

        public static async Task AddAsync(TTCheck check)
        {
            TTRepository repository = new TTRepository();

            await repository.SaveAsync(check);

            Month month = historic.SingleOrDefault(
                m => m.Check.DateTime.Month == check.DateTime.Month && m.Check.DateTime.Year == check.DateTime.Year);

            if (month != null)
            {
                Day day = month.Days.SingleOrDefault(d => d.Check.DateTime.Day == check.DateTime.Day);

                if (day != null)
                {
                    Hour hour = day.Hours.FirstOrDefault(h => h.Check.DateTime.Ticks < check.DateTime.Ticks);

                    int i = day.Hours.Count;

                    if (hour != null)
                        i = day.Hours.IndexOf(hour);                                       

                    day.Hours.Insert(i, new Hour(day.Hours)
                    {
                        Check = check
                    });
                }
                else
                    month.Days.Insert(0, new Day
                    {
                        Hours = getHours(new TTCheck[] { check }),
                        Check = check
                    });

            }
            else
                historic.Insert(0, new Month
                {
                    Check = check,
                    Days = new ObservableCollection<Day>(getDays(new TTCheck[] { check }))
                });
        }

        public static void AddDay(DateTime date)
        {
            Month month = historic.SingleOrDefault(
                m => m.Check.DateTime.Month == date.Month && date.Year == date.Year);

            if (month != null && !month.Days.Any(d => d.Check.DateTime.Day == date.Day))
            {
                Day day = month.Days.FirstOrDefault(d => d.Check.DateTime.Day < date.Day);

                int i = month.Days.Count;

                if (day != null)
                    i = month.Days.IndexOf(day);

                month.Days.Insert(i, new Day { Check = new TTCheck { UserName = string.Empty, DateTime = date } });
            }
        }
    }

}
