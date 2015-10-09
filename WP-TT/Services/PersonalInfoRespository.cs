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
    class PersonalInfoRespository
    {
        private const string FILENAME = "personal_info.txt";

        private async Task<StorageFile> LocalFile()
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

        public async Task SaveAsync(PersonalInfo personalInfo)
        {
            var file = await LocalFile();
            var jsonString = JsonConvert.SerializeObject(personalInfo);
            await FileIO.WriteTextAsync(file, jsonString);
        }

        public async Task<PersonalInfo> LoadAsync()
        {
            try
            {
                var file = await LocalFile();
                var jsonString = await FileIO.ReadTextAsync(file);
                var checks = JsonConvert.DeserializeObject<PersonalInfo>(jsonString);
                return checks;
            }
            catch
            {
                return null;
            }
        }
    }
}
