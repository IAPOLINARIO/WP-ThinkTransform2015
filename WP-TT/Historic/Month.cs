using System;
using System.Collections.ObjectModel;
using WP_TT.Models;

namespace WP_TT.Historic
{
    public class Month : HistoricItem
    {
        public ObservableCollection<Day> Days { get; set; }
    }
}
