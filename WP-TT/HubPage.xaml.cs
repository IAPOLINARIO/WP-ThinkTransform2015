using WP_TT.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WP_TT.Services;
using WP_TT.Models;
using System.Collections.ObjectModel;

namespace WP_TT
{
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private DispatcherTimer timer = new DispatcherTimer();

        private long gap;

        public HubPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            DateTime date = DateTime.Now.Add(TimeSpan.FromTicks(App.Gap));
            secondHand.Angle = date.Second * 6;
            minuteHand.Angle = date.Minute * 6;
            hourHand.Angle = (date.Hour * 30) + (date.Minute * 0.5);
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (!SecurityService.IsLogged)
            {
                GoToLoginPage();
            }
            else
            {
                var repository = new PersonalInfoRespository();
                var personalInfo = await repository.LoadAsync();
                HubSectionProfile.DataContext = personalInfo;

                var ttRepository = new TTRepository();
                var storedHistoricalChecks = await ttRepository.FindAllByUserNameAsync(personalInfo.login);

                int daysToCheck;
                if (DateTime.Now.Day > 20)
                {  // do dia 21 ao 20 do próximo mês
                    daysToCheck = DateTime.Now.Day - 20;
                    
                }
                else
                { // do dia 21 do mês passado ao dia 20 do mês atual
                    var day20 =  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 20).AddMonths(-1);
                    daysToCheck = (int) DateTime.Now.Subtract(day20).TotalDays;
                }

                var historicalChecks = new List<TTCheck>(storedHistoricalChecks);
                for (var i = 0; i < daysToCheck; i++)
                {
                    var day = DateTime.Now.AddDays(-i);
                    if ( ! day.IsWeekend() && ! storedHistoricalChecks.Any( s => s.DateTime.Year == day.Year &&
                        s.DateTime.Month == day.Month && 
                        s.DateTime.Day == day.Day
                        ))
                    {
                        historicalChecks.Add(new TTCheck() { DateTime = day, UserName = String.Empty });
                    }
                }

                var historicalChecksGroupedByMonthAndYear = historicalChecks
                    .OrderByDescending(s => s.DateTime)
                    .GroupBy(y => y.DateTime.Month.ToString() + y.DateTime.Year.ToString())
                    .Select(y => new
                    {
                        month = y.First().DateTime,
                        checksGroupedByDay = y.GroupBy(s => s.DateTime.Day)
                            .Select(d => new
                            {
                                day = d.First().DateTime,
                                color = (d.First().UserName == String.Empty) ? "#FC183C" : (d.Count() % 2 == 0 ? "#3FCED6" : "#FFD05F"),
                                collection = (d.First().UserName == String.Empty) ? null : d.Select(h => new
                                {
                                    hour = h.DateTime,
                                    color = d.Count() % 2 == 0 ? "#3FCED6" : "#FFD05F"
                                })
                            })
                    })
                    .ToArray();
                HubSectionHistoricalChecks.DataContext = historicalChecksGroupedByMonthAndYear;
            }
        }

        private void GoToLoginPage()
        {
            this.Frame.Navigate(typeof(Login));
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void LogoffButtonClicked(object sender, RoutedEventArgs e)
        {
            SecurityService.logoff();
            GoToLoginPage();
        }
    }
}
