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

        public HubPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.GetSyncDate().PropertyChanged += SyncDate_PropertyChanged;
        }

        private void SyncDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DateTime date = ((SyncDate)sender).Value;
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
                try
                {
                    var repository = new PersonalInfoRespository();
                    var personalInfo = await repository.LoadAsync();
                    HubSectionProfile.DataContext = personalInfo;

                    var builder = new HistoricalChecksArrayBuilder();
                    var historicalChecksGroupedByMonthAndYear = await builder.build(personalInfo);
                    HubSectionHistoricalChecks.DataContext = historicalChecksGroupedByMonthAndYear;
                }
                catch
                {
                    SecurityService.logoff();
                    GoToLoginPage();
                }
                
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
