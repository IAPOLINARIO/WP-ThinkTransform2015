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
        public async Task<DateTime?> CheckInOrOutAsync(string userName, string password)
        {
            var client = new TTClient();
            var checkinDatetime = await client.DoCheckInOrOutAsync(userName, password);

            if (checkinDatetime.HasValue)
            {
                var repository = new TTRepository();
                var check = new TTCheck();
                check.UserName = userName;
                check.DateTime = checkinDatetime.Value;
                await repository.SaveAsync(check);
            }

            return checkinDatetime;
        }
    }
}
