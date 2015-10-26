using WP_TT.Common;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WP_TT.Services;
using Windows.UI.Xaml.Controls.Primitives;
using WP_TT.Historic;
using Windows.UI.Xaml.Input;
using System;
using WP_TT.Models;

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

#if DEBUG
            this.debugMode.Visibility = Visibility.Visible;
#endif
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
                    PersonalInfoRespository repository = new PersonalInfoRespository();

                    HubSectionProfile.DataContext = await repository.LoadAsync();

                    HubSectionHistoricalChecks.DataContext = await Historic.Historic.Get();
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
            this.Frame.BackStack.Clear();
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

        private void showFlyout(object sender)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

            flyoutBase.ShowAt(senderElement);
        }

        private void Month_Holding(object sender, HoldingRoutedEventArgs e)
        {
            showFlyout(sender);
        }

        private void Day_Holding(object sender, HoldingRoutedEventArgs e)
        {
            showFlyout(sender);
        }

        private void Hour_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            showFlyout(sender);
        }

        private async void DeleteHourFlyout_Click(object sender, RoutedEventArgs e)
        {
            await ((Hour)((MenuFlyoutItem)sender).DataContext).Delete();
        }

        private void AddDayFlyout_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            DatePickerFlyout picker = new DatePickerFlyout();
            picker.ShowAt(senderElement);
            picker.DatePicked += Picker_DatePicked;
        }

        private void Picker_DatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            Historic.Historic.AddDay(args.NewDate.DateTime);
        }

        private void AddHourFlyout_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            Day day = (Day)senderElement.DataContext;

            TimePickerFlyout picker = new TimePickerFlyout();
            picker.ShowAt(senderElement);

            picker.TimePicked += async (s, a) =>
            {
                DateTime date = day.Check.DateTime.Date.AddTicks(a.NewTime.Ticks);
                await Historic.Historic.AddAsync(new TTCheck { DateTime = date, UserName = SecurityService.getCredential().Item1 });
            };
        }
    }
}
