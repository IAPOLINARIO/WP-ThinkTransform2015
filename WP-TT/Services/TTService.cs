using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WP_TT.Models;

namespace WP_TT.Services
{
    class TTService
    {
        public async Task<DateTime?> CheckInOrOutAsync()
        {
            var client = new TTClient();
                        
#if DEBUG
            var checkinDatetime = await this.doCheckInOrOutMockAsync();
#else
            var checkinDatetime = await client.DoCheckInOrOutAsync(SecurityService.getCredential().Item1, SecurityService.getCredential().Item2);
#endif

            if (checkinDatetime.HasValue)
            {
                var repository = new TTRepository();
                var check = new TTCheck();
                check.UserName = SecurityService.getCredential().Item1;
                check.DateTime = checkinDatetime.Value;
                await Historic.Historic.AddAsync(check);
            }

            return checkinDatetime;
        }

        //Only used in "debug mode"
        private async Task<DateTime?> doCheckInOrOutMockAsync()
        {
            await Task.Delay(1500);
            return (DateTime?)DateTime.Now;
        }
    }
}
