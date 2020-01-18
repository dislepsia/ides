using cnrl.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl
{
    public partial class AspNetUsers
    {
        public string descripcion
        {
            get { return Constantes.descripcionCompleja(this); }
        }

        public string TipoAlumnoDescripcion
        {
            get {

                return Logica.App.DisplayCamelCaseString(Enum.GetValues(typeof(TiposAlumno)).GetValue(TipoAlumno).ToString()); }
        }
    }
    
}

