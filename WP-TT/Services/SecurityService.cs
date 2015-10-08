using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace WP_TT.Services
{
    public class SecurityService
    {
        private const string loginURL = "https://people.cit.com.br/profile/{0}/";
        private const string photoURL = "https://people.cit.com.br/photos/{0}.jpg";
        private const string VAULT_RESOURCE = "TTCredentials";
        public static async Task<bool> tryLogin(string username, string password){
            
            Boolean success = false;
            try
            {
                Uri uri = new Uri(String.Format(photoURL, username));
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                filter.AllowUI = false;

                filter.ServerCredential =
                   new Windows.Security.Credentials.PasswordCredential(
                       uri.ToString(),
                       username,
                       password);

                HttpClient httpClient = new HttpClient(filter);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

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

        public static void logoff()
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
