using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl.ViewModels
{
    public class ViewNivelar
    {
        public cursa cursadaNivelar { get; set; }
        public List<curso> cursos { get; set; }

        public ViewNivelar()
        {
            cursos = new List<curso>();
        }
    }
}

