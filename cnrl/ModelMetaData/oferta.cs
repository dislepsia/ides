using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(ofertaMetadata))]
    public partial class oferta : IAuditable
    {
    }

    public class ofertaMetadata
    {

        [Required]
        public int curso { get; set; }
        [Required]
        public int periodoLectivo { get; set; }
        [Required]
        public bool habilitada { get; set; }
        [Required]
        public Nullable<System.TimeSpan> horaDesde { get; set; }
        [Required]
        public Nullable<System.TimeSpan> horaHasta { get; set; }
        [Required]
        public Nullable<int> sede { get; set; }
        [Required]
        public Nullable<System.DateTime> inscripcionDesde { get; set; }
        [Required]
        public Nullable<System.DateTime> inscripcionHasta { get; set; }
        [Required]
        public bool esBandaNegativa { get; set; }
        [Required]
        public double precioAlumno { get; set; }
        [Required]
        public double precioNoAlumno { get; set; }
        [Required]
        public double precioEmpleado { get; set; }
        [Required]
        public Nullable<double> descuentoBandaNegativa { get; set; }
        [Required]
        public Nullable<double> descuentoBandaNegativaNoAlumno { get; set; }
        [Required]
        public double precioMayorDe60 { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        public Nullable<double> descuentoUnaCuota { get; set; }
        [Required]
        public Nullable<int> cantidadCuotas { get; set; }
        [Required]
        public Nullable<int> cupo { get; set; }
        [Required]
        public Nullable<int> diasSegundoVencimiento { get; set; }
        [Required]
        public Nullable<double> recargoSegundoVencimiento { get; set; }
        /* [DataType(DataType.Date)]
         public Nullable<System.DateTime> fechaDocumentacion { get; set; }*/
        [Required]
        public Nullable<int> frecuenciaCuota { get; set; }
    }
}