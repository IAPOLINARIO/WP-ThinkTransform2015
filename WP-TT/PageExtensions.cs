using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WP_TT.Models;

namespace WP_TT
{
    public static class PageExtensions
    {
        public static SyncDate GetSyncDate(this Page page)
        {
            return (SyncDate)App.Current.Resources["SyncDate"];
        }
    }
}
