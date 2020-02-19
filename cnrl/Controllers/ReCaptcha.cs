using Newtonsoft.Json;
using System.Collections.Generic;

namespace cnrl.Controllers
{
    public class ReCaptcha
    {
        public static string Validar(string codigoValidacion)
        {
            var client = new System.Net.WebClient();

            string clavePrivada = "6Le0RNMUAAAAAOhKDByUgBKn_XlLh323-idtcT0s";

            var respuestaGoogle = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", clavePrivada, codigoValidacion));

            ReCaptcha respuesta = JsonConvert.DeserializeObject<ReCaptcha>(respuestaGoogle);

            return respuesta.Success;
        }

        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }


        private List<string> m_ErrorCodes;
    }
}