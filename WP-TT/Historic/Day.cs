using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WP_TT.Models;

namespace WP_TT.Historic
{
    public class Day : HistoricColoredItem
    {
        private ObservableCollection<Hour> hours;

        public ObservableCollection<Hour> Hours
        {
            get
            {
                return this.hours;
            }

            set
            {
                if (this.hours != null)
                    this.hours.CollectionChanged -= Hours_CollectionChanged;

                this.hours = value;

                if (this.hours != null)
                    this.Hours.CollectionChanged += Hours_CollectionChanged;

                this.colorize();
            }
        }

        public Day()
        {
            this.Hours = new ObservableCollection<Hour>();
        }

        private void Hours_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.colorize();
        }

        private void colorize()
        {
            if (!this.Hours.Any())
            {
                this.Style = HistoricColoredItemSytle.Empty;
            }
            else
            {
                this.Style = this.Hours.Count() % 2 == 0 ?
                    HistoricColoredItemSytle.Pair :
                    HistoricColoredItemSytle.Odd;
            }
        }
    }
}
