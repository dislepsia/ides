/* 
 * HttpCache vs Singleton - Best practice for an MVC application
 * http://stackoverflow.com/questions/13990623/httpcache-vs-singleton-best-practice-for-an-mvc-application 
 * 
*/

using cnrl.Models;
using cnrl.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace cnrl.Logica
{
    public class App
    {
        private static App _instancia;

        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public bool VerInactivos { get; set; }
        //private readonly int _paginado;
        //private readonly string _appVersion;
        //private readonly string _appBuilDate;
        //private readonly List<Nodo> _nodos;
        //private readonly List<CalidadModel> _calidad;
        //private readonly Dictionary<string, Media> _dic = new Dictionary<string, Media>();
        //private readonly Dictionary<string, DateTime> _inicios = new Dictionary<string, DateTime>();
        //private readonly Dictionary<string, IEnumerable<Tarea>> _tareas = new Dictionary<string, IEnumerable<Tarea>>();


        private App()
        {
            //Opciones opciones = OpcionesData.Get();
            //VerInactivos = opciones != null && opciones.VerInactivos;
            //// Grid
            ////_paginado = Convert.ToInt32(ConfigurationManager.AppSettings["grid.paginado"]);
            //// URLs
            //// nodo.medio.tipo.modo
            //// medio: {audio/video}
            //// tipo : {avisos/bloques}
            //// modo : {streaming/download}
            //// KEY
            //// nodo.medio.tipo
            //var claves = new [] {"{0}.audio.avisos", "{0}.video.avisos", "{0}.audio.bloques", "{0}.video.bloques"};
            //var nodos = ConfigurationManager.AppSettings["nodos"].Split(';');
            //foreach (var nodo in nodos)
            //{
            //    foreach (var clave in claves)
            //    {
            //        var key = string.Format(clave, nodo);
            //        var streaming = string.Format("{0}.streaming", key);
            //        var download = string.Format("{0}.download", key);
            //        _dic.Add(key, new Media { UrlStreaming = ConfigurationManager.AppSettings[streaming], UrlDownload = ConfigurationManager.AppSettings[download]});
            //    }
            //}
            //// Fechas de inicio
            //foreach (var nodo in nodos)
            //{
            //    var key = string.Format("{0}.fecha.inicio", nodo);
            //    var fecha = ConfigurationManager.AppSettings[key].ToString(CultureInfo.InvariantCulture); 
            //    var final = DateTime.ParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //    _inicios.Add(nodo, final);
            //}
            //// Build Version
            //_appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //_appVersion = _appVersion.Substring(0, _appVersion.Length - 2);
            //// Build Date
            //var path = HostingEnvironment.MapPath(@"~/App_Data/BuildDate.txt");
            //_appBuilDate = string.IsNullOrEmpty(path) ? "" : System.IO.File.ReadAllText(path);
            //// Nodos
            //_nodos = new List<Nodo>
            //{
            //    //new Nodo { IdNodo = "ARG", Descripcion = "Argentina"},
            //    new Nodo { IdNodo = "BRA", Descripcion = "Brasil"}
            //};
            //// Control de Calidad
            //_calidad = new List<CalidadModel>
            //{
            //    new CalidadModel { Id = 1, Descripcion = "Colecta sin Versión"			, DescripcionEn = "Colecta without Version"			, DescripcionPt = "Coleta sem Versão"			, SpDetalle = "COM_ValidarColectaSinTema" },
            //    new CalidadModel { Id = 2, Descripcion = "Colecta sin Programa"			, DescripcionEn = "Colecta without Program"			, DescripcionPt = "Coleta sem Programa"			, SpDetalle = "COM_ValidarColectaSinPrograma" },
            //    new CalidadModel { Id = 3, Descripcion = "Colecta sin Fechas"			, DescripcionEn = "Colecta without Dates"			, DescripcionPt = "Coleta sem Datas"			, SpDetalle = "COM_ValidarColectaSinFechas" },
            //    new CalidadModel { Id = 4, Descripcion = "Colecta sin Tarifario"		, DescripcionEn = "Colecta without Rate"			, DescripcionPt = "Coleta sem Tarifario"		, SpDetalle = "COM_ValidarColectaSinTarifario" },
            //    new CalidadModel { Id = 5, Descripcion = "Versión sin Producto"			, DescripcionEn = "Version without Product"			, DescripcionPt = "Versão sem Produto"			, SpDetalle = "COM_ValidarTemaSinProducto" },
            //    new CalidadModel { Id = 6, Descripcion = "Versión sin Colecta"			, DescripcionEn = "Version without Colecta"			, DescripcionPt = "Versão sem Coleta"			, SpDetalle = "COM_ValidarTemaSinColecta" },
            //    new CalidadModel { Id = 7, Descripcion = "Versión duplicada"			, DescripcionEn = "Duplicate Version"				, DescripcionPt = "Versão duplicada"			, SpDetalle = "COM_ValidarTemaDuplicado" },
            //    new CalidadModel { Id = 8, Descripcion = "Producto sin Marca"			, DescripcionEn = "Product without Brand"			, DescripcionPt = "Produto sem Marca"			, SpDetalle = "COM_ValidarProductoSinMarca" },
            //    new CalidadModel { Id = 9, Descripcion = "Producto sin Agencia"			, DescripcionEn = "Product without Agency"			, DescripcionPt = "Produto sem Agência"			, SpDetalle = "COM_ValidarProductoSinAgencia" },
            //    new CalidadModel { Id =10, Descripcion = "Producto sin Anunciante"		, DescripcionEn = "Product without Advertiser"		, DescripcionPt = "Produto sem Anunciante"		, SpDetalle = "COM_ValidarProductoSinAnunciante" },
            //    new CalidadModel { Id =11, Descripcion = "Producto sin Versión"			, DescripcionEn = "Product without Version"			, DescripcionPt = "Produto sem Versão"			, SpDetalle = "COM_ValidarProductoSinTema" },
            //    new CalidadModel { Id =12, Descripcion = "Producto sin Industria"		, DescripcionEn = "Product without Industry"		, DescripcionPt = "Produto sem Sétor"			, SpDetalle = "COM_ValidarProductoSinIndustria" },
            //    new CalidadModel { Id =13, Descripcion = "Producto sin Estructura"		, DescripcionEn = "Product without Structure"		, DescripcionPt = "Produto sem Item"			, SpDetalle = "COM_ValidarProductoSinEstructura" },
            //    new CalidadModel { Id =14, Descripcion = "Producto duplicado"			, DescripcionEn = "Duplicate Product"				, DescripcionPt = "Produto duplicado"			, SpDetalle = "COM_ValidarProductoDuplicado" },
            //    new CalidadModel { Id =15, Descripcion = "Marca sin Producto"			, DescripcionEn = "Brand without Product"			, DescripcionPt = "Marca sem Produto"			, SpDetalle = "COM_ValidarMarcaSinProducto" },
            //    new CalidadModel { Id =16, Descripcion = "Marca duplicada"				, DescripcionEn = "Duplicate Brand"					, DescripcionPt = "Marca duplicada"				, SpDetalle = "COM_ValidarMarcaDuplicada" },
            //    new CalidadModel { Id =17, Descripcion = "Agencia sin Producto"			, DescripcionEn = "Agency without Product"			, DescripcionPt = "Agência sem Produto"			, SpDetalle = "COM_ValidarAgenciaSinProducto" },
            //    new CalidadModel { Id =18, Descripcion = "Agencia duplicada"			, DescripcionEn = "Duplicate Agency"				, DescripcionPt = "Agência duplicada"			, SpDetalle = "COM_ValidarAgenciaDuplicada" },
            //    new CalidadModel { Id =19, Descripcion = "Agencia/Producto duplicado"	, DescripcionEn = "Duplicate Agency/Product"		, DescripcionPt = "Agência/Produto duplicado"	, SpDetalle = "COM_ValidarAgenciaProductoDuplicado" },
            //    new CalidadModel { Id =20, Descripcion = "Anunciante sin Producto"		, DescripcionEn = "Advertiser without Product"		, DescripcionPt = "Anunciante sem Produto"		, SpDetalle = "COM_ValidarAnuncianteSinProducto" },
            //    new CalidadModel { Id =21, Descripcion = "Anunciante duplicado"			, DescripcionEn = "Duplicate Advertiser"			, DescripcionPt = "Anunciante duplicado"		, SpDetalle = "COM_ValidarAnuncianteDuplicado" },
            //    new CalidadModel { Id =22, Descripcion = "Anunciante/Producto duplicado", DescripcionEn = "Duplicate Advertiser/Product"	, DescripcionPt = "Anunciante/Produto duplicado", SpDetalle = "COM_ValidarAnuncianteProductoDuplicado" },
            //    new CalidadModel { Id =23, Descripcion = "Programa duplicado"			, DescripcionEn = "Duplicate Program"				, DescripcionPt = "Programa duplicado"			, SpDetalle = "COM_ValidarProgramaDuplicado" },
            //    new CalidadModel { Id =24, Descripcion = "Programa sin relación"		, DescripcionEn = "Unrelated program"				, DescripcionPt = "Programa não relacionado"	, SpDetalle = "COM_ValidarProgramaSinRelacion" },
            //    new CalidadModel { Id =25, Descripcion = "Cotización del Dólar?"		, DescripcionEn = "Dollar Exchange Rate?"			, DescripcionPt = "Cotação do Dólar?"			, SpDetalle = "COM_ValidarTarifaDolarVigente" },
            //    new CalidadModel { Id =26, Descripcion = "Tarifario/Programa duplicado"	, DescripcionEn = "Duplicate Rates/Program"		    , DescripcionPt = "Tarifario/Programa duplicado", SpDetalle = "COM_ValidarTarifarioConProgramaDuplicado" }
            //};
        }


        public static App Singleton()
        {
            return _instancia ?? (_instancia = new App());

        }

        public static String sacarAcentos(string texto)
        {
            return texto.Replace('á', '_').Replace('é', '_').Replace('í', '_').Replace('ó', '_').Replace('ú', '_').Replace('Á', '_').Replace('É', '_').Replace('Í', '_').Replace('Ó', '_').Replace('Ú', '_');

        }

        //        busqueda = 

        public static string DisplayCamelCaseString(string camelCase)
        {
            List<char> chars = new List<char>();
            chars.Add(camelCase[0]);
            foreach (char c in camelCase.Skip(1))
            {
                int a;
                if (char.IsUpper(c) || int.TryParse(c.ToString(), out a))
                {
                    chars.Add(' ');
                    chars.Add(char.ToLower(c));
                }
                else
                    chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        public static void enviarMailUsuario(string eMail, string asunto, string cuerpo)
        {
            MailMessage mensaje = new MailMessage();
            mensaje.Subject = asunto;
            mensaje.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(cuerpo, null, MediaTypeNames.Text.Html));
            mensaje.HeadersEncoding = Encoding.UTF8;

            System.Net.NetworkCredential credentials;
            //            mensaje.Body = cuerpo;
            if (ApplicationUser.GetRol() != "9")
            {
                mensaje.From = new MailAddress(ConfigurationManager.AppSettings["ComunicacionesMail"]);
                credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["ComunicacionesMail"], ConfigurationManager.AppSettings["ComunicacionesPass"]);
            }
            else
            {
                mensaje.From = new MailAddress(ConfigurationManager.AppSettings["ComunicacionesMailGraduado"]);
                credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["ComunicacionesMailGraduado"], ConfigurationManager.AppSettings["ComunicacionesPass"]);
            }

            mensaje.To.Add(new MailAddress(eMail));
            mensaje.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["ComunicacionesSmtpDireccion"], int.Parse(ConfigurationManager.AppSettings["ComunicacionesSmtpPuerto"]));

            //smtpClient.Credentials = credentials;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential("SMTP_Injection", "59d20ee832b396152bd09349c00a09b537447450");
            smtpClient.EnableSsl = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            //smtpClient.EnableSsl = true;

            //ServicePointManager.ServerCertificateValidationCallback =
            //            delegate (object s, X509Certificate certificate,
            //                     X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //            { return true; };

            //try
            //{
            smtpClient.Send(mensaje);
            //}
            //catch (Exception ex)
            //{

            //}


        }

        public static string EnviarMailComunicacion(string eMail, string asunto, string cuerpo, bool cco, string remitente, HttpPostedFileBase adjunto1 = null, HttpPostedFileBase adjunto2 = null, HttpPostedFileBase adjunto3 = null, HttpPostedFileBase adjunto4 = null)
        {
            MailMessage mensaje = new MailMessage();
            mensaje.Subject = asunto;
            mensaje.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(cuerpo, null, MediaTypeNames.Text.Html));
            mensaje.HeadersEncoding = Encoding.UTF8;
            //adjuntos
            if (adjunto1 != null)
            {
                string fileName = Path.GetFileName(adjunto1.FileName);
                adjunto1.InputStream.Position = 0;
                var attachment = new Attachment(adjunto1.InputStream, fileName);
                mensaje.Attachments.Add(attachment);
            }
            if (adjunto2 != null)
            {
                string fileName = Path.GetFileName(adjunto2.FileName);
                adjunto2.InputStream.Position = 0;
                var attachment = new Attachment(adjunto2.InputStream, fileName);
                mensaje.Attachments.Add(attachment);
            }
            if (adjunto3 != null)
            {
                string fileName = Path.GetFileName(adjunto3.FileName);
                adjunto3.InputStream.Position = 0;
                var attachment = new Attachment(adjunto3.InputStream, fileName);
                mensaje.Attachments.Add(attachment);
            }
            if (adjunto4 != null)
            {
                string fileName = Path.GetFileName(adjunto4.FileName);
                adjunto4.InputStream.Position = 0;
                var attachment = new Attachment(adjunto4.InputStream, fileName);
                mensaje.Attachments.Add(attachment);
            }

            //            mensaje.Body = cuerpo;


            //mensaje.From = new MailAddress("afungueirino@anlis.gov.ar");
            if (ApplicationUser.GetRol() != "9")
            {
                mensaje.From = new MailAddress(ConfigurationManager.AppSettings["ComunicacionesMail"]);
            }
            else
            {
                mensaje.From = new MailAddress(ConfigurationManager.AppSettings["ComunicacionesMailGraduado"]);
            }
            
            //mensaje.HeadersEncoding = Encoding.UTF8;

            string mailsPrueba = ConfigurationManager.AppSettings["mails"];
            if (!string.IsNullOrEmpty(mailsPrueba)) eMail = mailsPrueba;

            var correos = eMail.Split(';');

            foreach (var correo in correos)
            {
                //    if (cco)
                //        mensaje.Bcc.Add(new MailAddress(correo.Trim()));
                //    else
                //        mensaje.CC.Add(new MailAddress(correo.Trim()));
                mensaje.To.Add(new MailAddress(correo));
                
            }

            mensaje.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["ComunicacionesSmtpDireccion"], int.Parse(ConfigurationManager.AppSettings["ComunicacionesSmtpPuerto"]));
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential("SMTP_Injection", "59d20ee832b396152bd09349c00a09b537447450");
            smtpClient.EnableSsl = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["ComunicacionesMail"], ConfigurationManager.AppSettings["ComunicacionesPass"]);
            //smtpClient.Credentials = credentials;
            //smtpClient.EnableSsl = true;

            //ServicePointManager.ServerCertificateValidationCallback =
            //            delegate (object s, X509Certificate certificate,
            //                     X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //            { return true; };

            //mensaje.IsBodyHtml = true;

            //SmtpClient clienteSmtp = new SmtpClient("smtp.sparkpostmail.com", 587);
            //clienteSmtp.Port = 587;
            //clienteSmtp.EnableSsl = true;
            //clienteSmtp.UseDefaultCredentials = true;
            //clienteSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //clienteSmtp.Credentials = new NetworkCredential("SMTP_Injection", "59d20ee832b396152bd09349c00a09b537447450");
            //clienteSmtp.EnableSsl = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            try
            {
                smtpClient.Send(mensaje);
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }


        public static bool UsuarioTienePermiso(string permiso)
        {
            socioculturalesEntities db = new socioculturalesEntities();
            string rol = ApplicationUser.GetRol();
            if (db.permiso.Where(x => x.nombre.Equals(permiso) && x.tipoUsuario.Equals(rol)).Any())
            {
                return true;
            }
            return false;
        }


    }

    //public enum Menu
    //{
    //    Comunidad = 1,
    //    Afiliado = 2,
    //    Nodo = 3,
    //    Admin = 4
    //}
}