using cnrl.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl
{
    public partial class periodolectivo
    {
        public string descripcion
        {
            get { return Constantes.descripcionCompleja(this); }
        }

    }
    
}

