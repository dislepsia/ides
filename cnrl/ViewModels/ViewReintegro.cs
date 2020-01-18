using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl.ViewModels
{
    public class ViewReintegro
    {

        public List<curso> cursoPersona { get; set; }

        public ViewReintegro ()
        {
            cursoPersona = new List<curso>();
        }
    }
}