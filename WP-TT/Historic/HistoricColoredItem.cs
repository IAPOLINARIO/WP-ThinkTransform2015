using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WP_TT.Models;

namespace WP_TT.Historic
{
    internal enum HistoricColoredItemSytle
    {
        Empty,
        Odd,
        Pair
    }

    public abstract class HistoricColoredItem : HistoricItem, INotifyPropertyChanged
    {
        private const string EMPTY_COLOR = "#FC183C";

        private const string ODD_COLOR = "#FFD05F";

        private const string PAIR_COLOR = "#3FCED6";

        private string color;

        public string Color
        {
            get
            {
                return this.color;
            }

            private set
            {
                this.color = value;
                this.OnPropertyChanged();
            }
        }

        private HistoricColoredItemSytle style;

        internal HistoricColoredItemSytle Style
        {
            get
            {
                return this.style;
            }
            set
            {
                this.style = value;
                switch (this.style)
                {
                    case HistoricColoredItemSytle.Empty:
                        this.Color = EMPTY_COLOR;
                        break;
                    case HistoricColoredItemSytle.Odd:
                        this.Color = ODD_COLOR;
                        break;
                    case HistoricColoredItemSytle.Pair:
                        this.Color = PAIR_COLOR;
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoricColoredItem()
        {
            this.Style = HistoricColoredItemSytle.Empty;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
