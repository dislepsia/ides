using cnrl.Logica;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace cnrl
{
    public partial class cursa
    {
        public string descripcion
        {

            get { return aprobo == true ? "Si" : "No"; }
        }

        public string nroReclamo
        {
            get { return codigo.ToString().PadLeft(10,'0'); }
        }
        //public string descripcionEstado
        //{
        //    get {

        //        return EstadoCursa.descripcion; }
        //}

        public DateTime fechaVencimientoCupon
        {
            get
            {
                int dias = int.Parse(ConfigurationManager.AppSettings["Dias1erCuota"].ToString());
                return DateModified.AddDays(dias);
            }
        }
    }
}

