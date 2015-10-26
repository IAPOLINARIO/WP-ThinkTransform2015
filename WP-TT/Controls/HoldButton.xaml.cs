using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WP_TT.Services;

namespace WP_TT.Controls
{

    public sealed partial class HoldButton : UserControl
    {

        public enum HoldButtonStatus
        {
            Idle,
            Holding,
            Waiting
        }

        public HoldButtonStatus Status { get; set; }

        private string topText;
        private string bottonText;

        public HoldButton()
        {
            this.InitializeComponent();
            this.Status = HoldButtonStatus.Idle;
            this.CompletedAnimation.Completed += CompletedAnimation_Completed;
            this.CanceledAnimation.Completed += CanceledAnimation_Completed;
        }

        private void CanceledAnimation_Completed(object sender, object e)
        {
            this.CheckAnimation.Stop();
            this.PulseAnimation.Stop();
            this.CanceledAnimation.Stop();
        }

        private void CompletedAnimation_Completed(object sender, object e)
        {
            this.PulseAnimation.Stop();
            this.CheckAnimation.Stop();
            this.CompletedAnimation.Stop();
        }

        private void btnCheckin_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (this.Status == HoldButtonStatus.Idle)
            {
                this.Status = HoldButtonStatus.Holding;

                this.Circle.Fill = new SolidColorBrush(Colors.Red);
                this.topText = ButtonTopText.Text;
                this.bottonText = ButtonBottomText.Text;
                this.ButtonTopText.Text = string.Empty;
                this.ButtonBottomText.Text = string.Empty;

                this.CheckAnimation.Begin();
            }
        }

        private void btnCheckin_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this.Status == HoldButtonStatus.Holding)
            {
                this.ButtonTopText.Text = this.topText;
                this.ButtonBottomText.Text = this.bottonText;

                if (String.IsNullOrEmpty(ButtonBottomText.Text))
                    this.ButtonBottomText.Visibility = Visibility.Collapsed;
                else
                    this.ButtonBottomText.Visibility = Visibility.Visible;

                this.Status = HoldButtonStatus.Idle;
                this.Circle.Fill = new SolidColorBrush(Colors.LightPink);
                this.GiveUpAnimation.Begin();
            }
        }

        private async void btnCheckin_Holding(object sender, HoldingRoutedEventArgs e)
        {
            switch (e.HoldingState)
            {
                case HoldingState.Started:
                    this.Status = HoldButtonStatus.Waiting;
                    this.PulseAnimation.Begin();
                    break;
                case HoldingState.Completed:
                    this.CompletedAnimation.Begin();

                    TTService service = new TTService();

                    this.ButtonTopText.Text = "waiting";
                    this.ButtonBottomText.Visibility = Visibility.Collapsed;

                    DateTime? date = await service.CheckInOrOutAsync();

                    if (date.HasValue)
                    {
                        this.ButtonTopText.Text = "CHECKED";
                        this.ButtonBottomText.Visibility = Visibility.Visible;
                        this.ButtonBottomText.Text = ((DateTime)date).ToString("HH:mm:ss");
                    }
                    else
                    {
                        this.CompletedAnimation.Begin();
                        this.ButtonTopText.Text = "try again";
                        this.ButtonBottomText.Visibility = Visibility.Collapsed;
                        await Task.Delay(500);
                        if (this.Status == HoldButtonStatus.Waiting)
                        {
                            this.ButtonTopText.Text = this.topText;
                            this.ButtonBottomText.Text = this.bottonText;
                            this.ButtonBottomText.Visibility = Visibility.Visible;
                        }
                    }

                    this.Status = HoldButtonStatus.Idle;
                    break;
                case HoldingState.Canceled:
                    this.ButtonTopText.Text = this.topText;
                    this.ButtonBottomText.Text = this.bottonText;

                    if (String.IsNullOrEmpty(this.ButtonBottomText.Text))
                        this.ButtonBottomText.Visibility = Visibility.Collapsed;
                    else
                        this.ButtonBottomText.Visibility = Visibility.Visible;

                    this.Status = HoldButtonStatus.Idle;
                    this.Circle.Fill = new SolidColorBrush(Colors.LightPink);
                    this.CanceledAnimation.Begin();
                    break;
            }
        }
    }
}
