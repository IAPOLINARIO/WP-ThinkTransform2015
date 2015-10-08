using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace WP_TT.Services
{
    internal class TTService
    {
        public async DateTime RemoteDatetime(){
            //var httpClient = new HttpClient();
            //var result = await httpClient.GetAsync(new Uri("https://tt.ciandt.com/.net/index.ashx/GetClockDeviceInfo?deviceID=2"));
            //var content = result.Content.ToString();
            //var regex = new Regex("dtTimeEvent: new Date\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");
            //var match = regex.Match();
            //match.
            return DateTime.Now;
        }

        public bool login(string login, string password)
        {
            return true;
        }

        public bool DoCheckIn(){
        // https://tt.ciandt.com/.net/index.ashx/GetClockDeviceInfo?deviceID=2
            return true;
        }

    }
}
