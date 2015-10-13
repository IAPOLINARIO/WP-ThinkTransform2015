using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace WP_TT
{
    public sealed partial class HoldButton : UserControl
    {
        public HoldButton()
        {
            this.InitializeComponent();
            startAnimation.Completed += StoryBoard_Completed;
            stopAnimation.Completed += StopAnimation_Completed;
        }

        private void StopAnimation_Completed(object sender, object e)
        {
            stopAnimation.Stop();
        }

        private void StoryBoard_Completed(object sender, object e)
        {
            if (started)
            {
                buttonTopText.Text = string.Empty;
                buttonBottomText.Text = string.Empty;
                buttonTopText.Text = "CHECKED";
                buttonBottomText.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            startAnimation.Stop();
            started = false;
        }

        private bool started;

        private void btnCheckin_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            canvasCircle.Fill = new SolidColorBrush(Colors.Red);
            buttonTopText.Text = "HOLD TO";
            buttonBottomText.Text = "CHECKIN";

            started = true;
            startAnimation.Begin();
            stopAnimation.Stop();
        }

        private void btnCheckin_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            canvasCircle.Fill = new SolidColorBrush(Colors.LightPink);

            if (started)
            {
                started = false;
                stopAnimation.Begin();
            }
        }

    }
}
