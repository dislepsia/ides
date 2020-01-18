using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(periodolectivoMetadata))]
    public partial class periodolectivo : IAuditable
    {

    }

    public partial class periodolectivoMetadata
    {
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int tipoCurso { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int anio { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Range(1, 5, ErrorMessage = "Periodo debe estar entre 1 y 5")]
        public int periodo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime desdeFecha { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime hastaFecha { get; set; }

        [DisplayFormat(DataFormatString = "{0:t}")]
        public Nullable<System.DateTime> periodoHoraDesde { get; set; }

        [DisplayFormat(DataFormatString = "{0:t}")]
        public Nullable<System.DateTime> periodoHoraHasta { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public Nullable<System.DateTime> fechaInscripcionHasta { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public Nullable<System.DateTime> fechaInscripcionDesde { get; set; }

        [DisplayFormat(DataFormatString = "{0:t}")]
        public Nullable<System.DateTime> inscripcionHoraDesde { get; set; }

        [DisplayFormat(DataFormatString = "{0:t}")]
        public Nullable<System.DateTime> inscripcionHoraHasta { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public Nullable<System.DateTime> fechaPrimeraCuota { get; set; }
    }
}