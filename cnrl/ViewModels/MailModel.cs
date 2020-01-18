using System.ComponentModel;
using System.Web.Mvc;

namespace cnrl.ViewModels
{
    public class MailModel
    {
        [DisplayName("Destinatarios del Email")]
        public string destinatarios { get; set; }
        public string modo { get; set; }

        [DisplayName("Asunto del Email")]
        public string asunto { get; set; }

        [DisplayName("Cuerpo del Email"), AllowHtml]
        public string editor                    { get; set; }
        [DisplayName("Copia Oculta")]
        public bool cco { get; set; }
        public bool morosos { get; set; }

        public int IdOferta { get; set; }

        public byte[] adjunto { get; set; }

        public string codigos { get; set; }
        public MailModel()
        {

        }
    }
}