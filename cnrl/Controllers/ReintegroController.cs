using cnrl.Logica;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Word = Microsoft.Office.Interop.Word;


namespace cnrl.Controllers
{
    public class ReintegroController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: Reintegro
        [HasPermission("Admin_Index")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reintegro/Details/5
        [HasPermission("Admin_Index")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reintegro/Create
        [HasPermission("Admin_Index")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reintegro/Create
        [HttpPost]
        public ActionResult Create(int? id)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reintegro/Edit/5
        [HasPermission("Admin_Index")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            //var curso = new curso();
            ViewBag.modo = "Edit";
            ViewBag.Callback = callback;

            AspNetUsers usuario = db.AspNetUsers.Where(x => x.NroDocumento == id).First();

            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View("Edit", usuario);
        }

        // POST: Reintegro/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reintegro/Delete/5
        [HasPermission("Admin_Index")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reintegro/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public FileResult Download(int? documento, string curso)
        {
            AspNetUsers persona = db.AspNetUsers.Where(x => x.NroDocumento == documento).First();

            string nombreYapellido = persona.Nombre + " " + persona.Apellido;
            string arancel = "";
            string recibo = "";

            #region Generar Nota

            //Objeto del Tipo Word Application 
            Word.Application objWordApplication;
            
            //Objeto del Tipo Word Document 
            Word.Document objWordDocument;
            // Objeto para interactuar con el Interop 
            Object oMissing = System.Reflection.Missing.Value;
            //Creamos una instancia de una Aplicación Word. 
            objWordApplication = new Word.Application();
            
            //A la aplicación Word, le añadimos un documento. 
            objWordDocument = objWordApplication.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                        
            //Activamos el documento recien creado, de forma que podamos escribir en el 
            objWordDocument.Activate();
            objWordDocument.PageSetup.PaperSize = Word.WdPaperSize.wdPaperA4;

            //Encabezado de pagina
            Word.Range imagen = objWordDocument.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            imagen.Font.Size = 24;
            imagen.Font.Bold = 0;

            var folder = Server.MapPath("~/Reintegros/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);

                string filePath = System.IO.Path.Combine(folder, "reintegros.txt");

                using (StreamWriter writer = System.IO.File.CreateText(filePath))
                {
                    writer.WriteLine("entra");


                }
            }

            if (User.IsInRole("AdministradorGraduado") == true)
            {
                imagen.Font.Name = "Palace Script MT";
                imagen.Text = "\nUniversidad Nacional de La Matanza\nDirección de Graduados";
                imagen.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                imagen.InlineShapes.AddPicture(HttpRuntime.AppDomainAppPath + "graduado.ico", false, true, imagen);
            }
            else
            {
                imagen.Font.Name = "Palace Script MT";
                imagen.Text = "\nUniversidad Nacional de La Matanza\nSecretaría de Extensión Universitaria";
                imagen.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                imagen.InlineShapes.AddPicture(HttpRuntime.AppDomainAppPath + "favicon.ico", false, true, imagen);
            }
            

            //Formato de texto
            objWordApplication.Selection.Font.Size = 12; //Tamaño de la Fuente 
            objWordApplication.Selection.Font.Bold = 1; // Negrita 
            objWordApplication.Selection.Font.Name = "Times New Roman";//Tipo de letra
            objWordApplication.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;

            string Escritura = "";
            if (User.IsInRole("AdministradorGraduado") == true)
            {
                Escritura = "Nota DG Nº\nSan Justo, " + System.DateTime.Now.Date.ToLongDateString() + "\nRef: Reintegro\n\n";
            }
            else
            {
                Escritura = "Nota SEU Nº\nSan Justo, " + System.DateTime.Now.Date.ToLongDateString() + "\nRef: Reintegro\n\n";
            }

            objWordApplication.Selection.TypeText(Escritura);

            //A quien corresponde
            objWordApplication.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            objWordApplication.Selection.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpace1pt5;
            Escritura = "\nSeñora Tesorera:\n";
            objWordApplication.Selection.TypeText(Escritura);

            //Pedido
            objWordApplication.Selection.ParagraphFormat.SpaceAfter = 0f;
            objWordApplication.Selection.Font.Bold = 0; // Negrita 
            Escritura = "\t\t\tSolicito a usted autorización para el reintegro del  arancel abonado, que " +
                        "posteriormente se detalla, en concepto del curso que no inició por falta del número mínimo de " +
                        "alumnos requeridos.\n";
            objWordApplication.Selection.TypeText(Escritura);

            //formato de texto
            objWordApplication.Selection.ParagraphFormat.SpaceAfter = 0f;
            Escritura = "\t\t\tA continuación se detallan los datos del alumno, arancel abonado y numero de recibo:\n\n";
            objWordApplication.Selection.TypeText(Escritura);

            //Fin de Nota
            Escritura = "\t\t\tSin otro particular, saludo a usted muy atentamente.\n";
            objWordApplication.Selection.TypeText(Escritura);

            objWordApplication.Selection.Font.Bold = 1; // Negrita 
            Escritura = "\n\n\n\n\n\n\n\n\nTesoreria\nS             /            D";
            objWordApplication.Selection.TypeText(Escritura);

            //Tabla
            string text = objWordDocument.Content.Text;
            int start = text.IndexOf("o:");
            int end = text.IndexOf("Sin otro");

            Word.Range rng = objWordApplication.ActiveDocument.Range(Start: start + 2, End: end);
            objWordApplication.Selection.Tables.Add(rng, 2, 5);

            //Cabecera
            objWordDocument.Tables[1].Cell(1, 1).Range.Text = "Apellido y Nombre";
            objWordDocument.Tables[1].Cell(1, 2).Range.Text = "DNI";
            objWordDocument.Tables[1].Cell(1, 3).Range.Text = "Arancel Abonado";
            objWordDocument.Tables[1].Cell(1, 4).Range.Text = "Nro. de Recibo";
            objWordDocument.Tables[1].Cell(1, 5).Range.Text = "Curso";
            //Relleno
            objWordDocument.Tables[1].Cell(2, 1).Range.Text = nombreYapellido;
            objWordDocument.Tables[1].Cell(2, 2).Range.Text = documento.ToString();
            objWordDocument.Tables[1].Cell(2, 3).Range.Text = arancel;
            objWordDocument.Tables[1].Cell(2, 4).Range.Text = recibo;
            objWordDocument.Tables[1].Cell(2, 5).Range.Text = curso;
            //Autoaujustar
            objWordDocument.Tables[1].Columns.AutoFit();
            //Lineas
            objWordDocument.Tables[1].Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            objWordDocument.Tables[1].Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            //Hace visible la Aplicacion para que veas lo que se ha escrito 
            objWordApplication.Visible = false;

            #endregion

           

            String ruta = HttpRuntime.AppDomainAppPath + "Reintegro.doc";
            
            string[] files = System.IO.Directory.GetFiles(HttpRuntime.AppDomainAppPath, "*.doc");

            


            foreach (string file in files)
                System.IO.File.Delete(file);

            objWordDocument.SaveAs2(ruta);
            objWordDocument.Close(ref oMissing, ref oMissing);
            objWordApplication.Quit(ref oMissing, ref oMissing);

            WebClient cliente = new WebClient();
            byte[] stream = cliente.DownloadData(ruta);
            //Word->vnd.openxmlformats - officedocument.wordprocessingml.document
            return File(stream, "application/Word-> vnd.openxmlformats-officedocument.wordprocessingml.document", "Reintegro.doc");
            //return RedirectToAction("Index", "Reintegro");

        }



        [HasPermission("Admin_Index")]
        public ActionResult PartialGrid(string texto)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(texto) && texto != Constantes.ERROR.ToString())
                {
                    var persona = db.AspNetUsers.Where(x => x.NroDocumento.Value.ToString() == texto).ToList();

                    if (persona.Count > 0)
                    {
                        return PartialView("_Grid", User.IsInRole("AdministradorEmpresas") == true ? persona.First().cursa.Where(x => x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Empresas).ToList() : User.IsInRole("AdministradorGraduado") == true ? persona.First().cursa.Where(x => x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Graduados).ToList() : persona.First().cursa.Where(x => x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Idiomas).ToList());
                    }
                }
            }

            return PartialView("_Grid", null);
        }
    }
}
