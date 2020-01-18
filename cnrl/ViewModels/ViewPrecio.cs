using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl.ViewModels
{
    public class ViewPrecio
    {
        public List<precio> precios { get; set; }

        public ViewPrecio()
        {
            precios = new List<precio>();
        }
    }
}

