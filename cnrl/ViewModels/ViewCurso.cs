using cnrl.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl
{
    public partial class curso
    {
        public string descripcionCompleja
        {
            get { return Constantes.descripcionCompleja(this); }
        }

    }
    
}

