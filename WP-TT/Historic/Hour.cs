using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WP_TT.Models;
using WP_TT.Services;
using System.Linq;

namespace WP_TT.Historic
{
    public class Hour : HistoricColoredItem
    {
        public async Task Delete()
        {
            TTRepository repository = new TTRepository();

            await repository.DeleteAsync(this.Check);

            this.Hours.Remove(this);
        }

        public ObservableCollection<Hour> Hours { get; private set; }

        public Hour(ObservableCollection<Hour> hours)
        {
            this.Hours = hours;
            this.Style = HistoricColoredItemSytle.Odd;
            this.Hours.CollectionChanged += Hours_CollectionChanged;
        }

        private void Hours_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Style = this.Hours.Count() % 2 == 0 ?
                HistoricColoredItemSytle.Pair :
                HistoricColoredItemSytle.Odd;
        }

    }
}
