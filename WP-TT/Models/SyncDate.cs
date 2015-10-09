using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using WP_TT.Services;

namespace WP_TT.Models
{
    public class SyncDate : INotifyPropertyChanged
    {
        private long gap;

        private DispatcherTimer timer = new DispatcherTimer();

        public SyncDate()
        {
            OnPropertyChanged("Value");

            var service = new TTClient();

            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += (s, e) =>
            {
                OnPropertyChanged("Value");
            };

            timer.Start();

            service.RemoteDatetimeAsync().ContinueWith(t =>
            {
                if (t.IsCompleted)
                {
                    gap = t.Result.Subtract(DateTime.Now).Ticks;
                }
            });
        }

        public DateTime Value {
            get
            {
                return DateTime.Now.Add(TimeSpan.FromTicks(gap));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}