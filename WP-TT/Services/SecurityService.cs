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


    }
}
