using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using WP_TT.Models;

namespace WP_TT.Services
{
    public class SecurityService
    {
        private const string loginURL = "https://people.cit.com.br/profile/{0}?format=json";
        private const string photoURL = "https://people.cit.com.br/photos/{0}.jpg";
        private const string VAULT_RESOURCE = "TTCredentials";
        public static async Task<bool> tryLogin(string username, string password){
            
            Boolean success = false;
            try
            {
                Uri uri = new Uri(String.Format(loginURL, username));
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                filter.AllowUI = false;

                filter.ServerCredential =
                   new Windows.Security.Credentials.PasswordCredential(
                       uri.ToString(),
                       username,
                       password);

                var httpClient = new HttpClient(filter);

                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var jsonString = response.Content.ToString();
                var result = JsonConvert.DeserializeAnonymousType(jsonString, new { personal_info = new PersonalInfo() });
                var personalPhotoUrl = String.Format(photoURL, username);
                var photoHttpResponse = await httpClient.GetAsync(new Uri(personalPhotoUrl));
                
                var repository = new PersonalInfoRespository();
                IBuffer buffer = await photoHttpResponse.Content.ReadAsBufferAsync();
                String photo = await repository.SavePhotoAsync(buffer, username);
                result.personal_info.photo = photo;
                await repository.SaveAsync(result.personal_info);

                var vault = new PasswordVault();
                vault.Add(new PasswordCredential(VAULT_RESOURCE, username, password));
                success = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);  
            }
            
            return success;
        }

        public static Tuple<string, string> getCredential()
        {
            var vault = new PasswordVault();

            Tuple<string, string> credential = null;

            try {
                var creds = vault.FindAllByResource(VAULT_RESOURCE).FirstOrDefault();
                if (creds != null)
                {
                    String username = creds.UserName;
                    String password = vault.Retrieve(VAULT_RESOURCE, username).Password;
                    credential = new Tuple<string, string>(username, password);
                }
                
            } catch (Exception e){

            }

            return credential;
           
        }

        public static bool IsLogged
        {
            get
            {
                var vault = new PasswordVault();
                try
                {
                    var creds = vault.FindAllByResource(VAULT_RESOURCE).FirstOrDefault();
                    if (creds != null)
                    {
                        return true;
                    }
                }
                catch {}
                return false;
            }
        }

        public static void logoff()
        {
            if (IsLogged)
            {
                var vault = new PasswordVault();
                try
                {
                    var creds = vault.FindAllByResource(VAULT_RESOURCE).FirstOrDefault();
                    if (creds != null)
                    {
                        vault.Remove(creds);
                    }

                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
