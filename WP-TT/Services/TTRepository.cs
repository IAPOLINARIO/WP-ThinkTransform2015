using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using WP_TT.Models;

namespace WP_TT.Services
{

    class TTRepository
    {
        private const string FILENAME = "checks.txt";
        public async Task SaveAsync(TTCheck check)
        {
            var checks = (await FindAllAsync()).ToList();
            checks.Add(check);
            var jsonString = JsonConvert.SerializeObject(checks);
            var file = await LocalFile();
            await FileIO.WriteTextAsync(file, jsonString);
        }

        private async Task<StorageFile> LocalFile()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(FILENAME, CreationCollisionOption.ReplaceExisting);
            return file;
        }

        private async Task<IEnumerable<TTCheck>> FindAllAsync()
        {
            try
            {
                var file = await LocalFile();
                var jsonString = await FileIO.ReadTextAsync(file);
                var checks = JsonConvert.DeserializeObject<List<TTCheck>>(jsonString);
                return checks;
            }
            catch
            {
                return Enumerable.Empty<TTCheck>();
            }
        }

        public async Task<IEnumerable<TTCheck>> FindAllByUserNameAsync(string userName)
        {
            var checks = await FindAllAsync();
            var checksByUserName = checks.Where(s => s.UserName == userName);
            return checksByUserName.ToArray();
        }
    }
}
