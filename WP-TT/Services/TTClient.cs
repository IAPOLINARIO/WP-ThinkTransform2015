using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace WP_TT.Services
{
    internal class TTClient
    {
        private const string BASE_URL = "https://tt.ciandt.com/.net/index.ashx/";
        private Uri baseUri;

        private const int CHECK_RECORDED = 1;
        private const int INVALID_CREDENTIALS = 2;
        private const int OFFLINE_BACKEND = 3;

        internal TTClient()
        {
            baseUri = new Uri(BASE_URL);
        }

        private DateTime ExtractDatetimeFromRemoteDatetimeHttpResponse(HttpResponseMessage result)
        {
            var content = result.Content.ToString();
            var regex = new Regex(@"dtTimeEvent: new Date\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");
            var match = regex.Match(content);
            var year = int.Parse(match.Groups[1].Value);
            var month = int.Parse(match.Groups[2].Value) + 1;
            var day = int.Parse(match.Groups[3].Value);
            var hour = int.Parse(match.Groups[4].Value);
            var minutes = int.Parse(match.Groups[5].Value);
            var seconds = int.Parse(match.Groups[6].Value);

            return new DateTime(year, month, day, hour, minutes, seconds);
        }

        public async Task<DateTime> RemoteDatetimeAsync()
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(new Uri(baseUri, "GetClockDeviceInfo?deviceID=2"));
            return ExtractDatetimeFromRemoteDatetimeHttpResponse(result);
        }

        public string FixJson(string script)
        {
            Regex regexFixJson = new Regex("(\\s*?{\\s*?|\\s*?,\\s*?)(['\"])?([a-zA-Z0-9_]+)(['\"])?:");

            string fixedJson = regexFixJson.Replace(script, "$1\"$3\":");

            return fixedJson;
        }

        public async Task<DateTime?> DoCheckInOrOutAsync(string userName, string password)
        {
            Regex regexSuccessCheckIn = new Regex(@"success:\s*true");
            Uri checkInUri = new Uri(baseUri, "SaveTimmingEvent");
            var httpClient = new HttpClient();
            DateTime checkinDateTime = await RemoteDatetimeAsync();

            try
            {
                Dictionary<string, string> contentParameters = new Dictionary<string, string>
                {
                    {"deviceID", "2"},
                    {"eventType", "1"},
                    {"userName", userName},
                    {"password", password},
                    {"cracha", ""},
                    {"costCenter", ""},
                    {"leave", ""},
                    {"func", ""},
                    {"cdiDispositivoAcesso", "2"},
                    {"cdiDriverDispositivoAcesso", "10"},
                    {"cdiTipoIdentificacaoAcesso", "7"},
                    {"oplLiberarPETurmaRVirtual", "false"},
                    {"cdiTipoUsoDispositivo", "1"},
                    {"qtiTempoAcionamento", "0"},
                    {"d1sEspecieAreaEvento", "Nenhuma"},
                    {"d1sAreaEvento", "Nenhum"},
                    {"d1sSubAreaEvento", "Nenhum(a)"},
                    {"d1sEvento", "Nenhum"},
                    {"oplLiberarFolhaRVirtual", "false"},
                    {"oplLiberarCCustoRVirtual", "false"},
                    {"qtiHorasFusoHorario", "0"},
                    {"cosEnderecoIP", "127.0.0.1"},
                    {"nuiPorta", "7069"},
                    {"oplValidaSenhaRelogVirtual", "false"},
                    {"useUserPwd", "true"},
                    {"useCracha", "false"},
                    {"dtTimeEvent", ""},
                    {"oplLiberarFuncoesRVirtual", "false"},
                    {"sessionID", "0"},
                    {"selectedEmployee", "0"},
                    {"selectedCandidate", "0"},
                    {"selectedVacancy", "0"},
                    {"dtFmt", "d/m/Y"},
                    {"tmFmt", "H:i:s"},
                    {"shTmFmt", "H:i"},
                    {"dtTmFmt", "d/m/Y H:i:s"},
                    {"language", "0"}
                };


                var content = new Windows.Web.Http.HttpFormUrlEncodedContent(contentParameters);

                HttpResponseMessage response = await httpClient.PostAsync(checkInUri, content);

                string responseContent = await response.Content.ReadAsStringAsync();
                string responseJson = FixJson(responseContent);

                var responseType = new { success = false, msg = new { type = 0, msg = "" } };

                var responseObject = JsonConvert.DeserializeAnonymousType(responseJson, responseType);

                bool result = false;

                switch ((int)responseObject.msg.type)
                {
                    case CHECK_RECORDED:
                        result = true;
                        break;
                    case INVALID_CREDENTIALS:
                    case OFFLINE_BACKEND:
                        result = false;
                        break;
                }

                if (result && responseObject.success)
                {
                    return checkinDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
