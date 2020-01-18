//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cnrl
{
    using System;
    using System.Collections.Generic;
    
    public partial class oferta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public oferta()
        {
            this.cursa = new HashSet<cursa>();
        }
    
        public int curso { get; set; }
        public int periodoLectivo { get; set; }
        public System.DateTime fechaDesde { get; set; }
        public System.DateTime fechaHasta { get; set; }
        public bool habilitada { get; set; }
        public int codigo { get; set; }
        public bool lunes { get; set; }
        public bool martes { get; set; }
        public bool miercoles { get; set; }
        public bool jueves { get; set; }
        public bool viernes { get; set; }
        public bool sabado { get; set; }
        public Nullable<System.TimeSpan> horaDesde { get; set; }
        public Nullable<System.TimeSpan> horaHasta { get; set; }
        public Nullable<int> sede { get; set; }
        public Nullable<System.DateTime> inscripcionDesde { get; set; }
        public Nullable<System.DateTime> inscripcionHasta { get; set; }
        public Nullable<System.DateTime> fechaPrimerCuota { get; set; }
        public bool esBandaNegativa { get; set; }
        public Nullable<System.TimeSpan> horaInscripcionDesde { get; set; }
        public Nullable<System.TimeSpan> horaInscripcionHasta { get; set; }
        public string docente { get; set; }
        public string coordinadorDocente { get; set; }
        public double precioAlumno { get; set; }
        public double precioNoAlumno { get; set; }
        public double precioEmpleado { get; set; }
        public Nullable<double> descuentoBandaNegativa { get; set; }
        public double precioMayorDe60 { get; set; }
        public Nullable<double> descuentoUnaCuota { get; set; }
        public Nullable<int> cantidadCuotas { get; set; }
        public Nullable<int> cupo { get; set; }
        public Nullable<int> diasSegundoVencimiento { get; set; }
        public Nullable<double> recargoSegundoVencimiento { get; set; }
        public Nullable<double> recaudacionMinima { get; set; }
        public Nullable<int> cupoMinimo { get; set; }
        public bool RequierePreInscripcion { get; set; }
        public string documentacion { get; set; }
        public Nullable<System.DateTime> fechaDocumentacion { get; set; }
        public Nullable<System.TimeSpan> horaDocumentacion { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
        public string UserCreated { get; set; }
        public string UserModified { get; set; }
        public bool gratuito { get; set; }
        public Nullable<int> frecuenciaCuota { get; set; }
        public double precioGraduado { get; set; }
        public Nullable<double> descuentoBandaNegativaNoAlumno { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cursa> cursa { get; set; }
        public virtual curso curso1 { get; set; }
        public virtual curso curso2 { get; set; }
        public virtual frecuenciaCuota frecuenciaCuota1 { get; set; }
        public virtual periodolectivo periodolectivo1 { get; set; }
        public virtual periodolectivo periodolectivo2 { get; set; }
        public virtual sede sede1 { get; set; }
        public virtual sede sede2 { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
    }
}
