using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WP_TT
{
    public sealed partial class DigitalWatchControl : UserControl
    {
        DependencyProperty DateTimeProperty = DependencyProperty.Register("DateTime", typeof(DateTime), typeof(DigitalWatchControl), new PropertyMetadata(DateTime.Now));

        public DateTime DateTime
        {
            get
            {
                return (DateTime)GetValue(DateTimeProperty);
            }
            set
            {
                SetValue(DateTimeProperty, value);
            }
        }

        private TextBlock hours;

        private TextBlock seconds;

        private TextBlock date;

        public DigitalWatchControl()
        {
            this.InitializeComponent();
        }
    }

    public class DigitalWatchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int param = int.Parse(parameter.ToString());

            DateTime date = (DateTime)value;

            switch (param)
            {
                case 0 : return date.ToString("HH:mm");
                case 1 : return String.Format(":{0:00}", date.Second);
                case 2 : return date.ToString("dd MMMM yyyy");
                default: return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
