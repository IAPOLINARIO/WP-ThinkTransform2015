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
        public async Task<DateTime> RemoteDatetime(){
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(new Uri("https://tt.ciandt.com/.net/index.ashx/GetClockDeviceInfo?deviceID=2"));
            var content = result.Content.ToString();
            var regex = new Regex(@"dtTimeEvent: new Date\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");
            var match = regex.Match(content);
            var year = int.Parse(match.Groups[1].Value);
            var month = int.Parse(match.Groups[2].Value);
            var day = int.Parse(match.Groups[3].Value);
            var hour = int.Parse(match.Groups[4].Value);
            var minutes = int.Parse(match.Groups[5].Value);
            var seconds = int.Parse(match.Groups[6].Value);

            return new DateTime(year, month, day, hour, minutes, seconds);
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
