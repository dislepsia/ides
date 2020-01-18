using cnrl.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cnrl
{
    public partial class oferta
    {
        public string descripcion
        {
            get { return Constantes.descripcionCompleja(this); }
        }

        public string horario
        {
            get { return Constantes.descripcionHorario(this); }
        }
        public int cantidadInscriptos
        {
            get { return this.cursa.Where(x => x.estado == (int)EstadosCursada.FormaPago || x.estado == (int)EstadosCursada.Confirmado || x.estado == (int)EstadosCursada.PlanGenerado || x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.PreInscripcion || x.estado == (int)EstadosCursada.Migrado).Count(); }
        }

        public int cantidadPreInscriptos
        {
            get { return this.cursa.Where(x => x.estado == (int)EstadosCursada.PreInscripcion).Count(); }
        }

        public int cantidadInscriptosConfirmados
        {

            get { return this.cursa.Where(x => x.estado == (int)EstadosCursada.Confirmado || x.estado == (int)EstadosCursada.FormaPago).Count(); }
        }
        public decimal importeInscritos
        {
            get; set;
        }
    }


}

