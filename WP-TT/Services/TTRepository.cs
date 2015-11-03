using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using WP_TT.Models;

namespace WP_TT.Services
{

    public class TTRepository
    {
        private const string FILENAME = "checks.txt";

        public async Task SaveAsync(TTCheck check)
        {
            var checks = (await findAllAsync()).ToList();
            checks.Add(check);
            var jsonString = JsonConvert.SerializeObject(checks);
            var file = await localFile();
            await FileIO.WriteTextAsync(file, jsonString);
        }

        public async Task DeleteAsync(TTCheck check)
        {
            var checks = (await findAllAsync()).ToList();

            TTCheck orginalCheck = checks.Single(c => c.UserName == check.UserName && c.DateTime.Ticks == check.DateTime.Ticks);

            checks.Remove(orginalCheck);

            var jsonString = JsonConvert.SerializeObject(checks);
            var file = await localFile();
            await FileIO.WriteTextAsync(file, jsonString);
        }

        private async Task<StorageFile> localFile()
        {
            StorageFile file;

            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(FILENAME);
            }
            catch
            {
                file = ApplicationData.Current.LocalFolder.CreateFileAsync(FILENAME, CreationCollisionOption.ReplaceExisting).GetResults();
            }

            return file;
        }

        private async Task<IEnumerable<TTCheck>> findAllAsync()
        {
            try
            {
                var file = await localFile();
                var jsonString = await FileIO.ReadTextAsync(file);
                if (jsonString != String.Empty)
                {
                    var checks = JsonConvert.DeserializeObject<List<TTCheck>>(jsonString);
                    return checks;
                }
            }
            catch
            {
            }

            return Enumerable.Empty<TTCheck>();
        }

        public async Task<IEnumerable<TTCheck>> FindAllByUserNameAsync(string userName)
        {
            var checks = await findAllAsync();
            var checksByUserName = checks.Where(s => s.UserName == userName);

            return checksByUserName.ToArray();
        }
    }
}
