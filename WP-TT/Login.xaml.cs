using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WP_TT.Services;
using Windows.UI.Popups;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WP_TT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public Login()
        {
            this.InitializeComponent();
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {

            /*ImageBrush imageBrush = new ImageBrush();
            Uri uri = new Uri(@"Assets/login_press.png", UriKind.Relative);
            imageBrush.ImageSource = new BitmapImage(uri);
            loginButton.Background = imageBrush;*/
            progressRing.IsActive = true;
            
            bool result = await SecurityService.tryLogin(usernameTextBox.Text, passwordTextBox.Password);
            progressRing.IsActive = false;
            if (result)
            {
                this.Frame.Navigate(typeof(HubPage));
            }
            else
            {
                MessageDialog message = new MessageDialog("Usuário ou senha inválidos");
                
                message.ShowAsync();
            }

        }
    }
}
