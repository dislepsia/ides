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
    
    public partial class sede
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sede()
        {
            this.oferta = new HashSet<oferta>();
            this.oferta1 = new HashSet<oferta>();
            this.AspNetUsers = new HashSet<AspNetUsers>();
        }
    
        public int codigo { get; set; }
        public string nombre { get; set; }
        public string domicilio { get; set; }
        public string telefono { get; set; }
        public string mail { get; set; }
        public string contacto { get; set; }
        public Nullable<bool> borradoLogico { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
        public string UserCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<int> sector { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<oferta> oferta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<oferta> oferta1 { get; set; }
        public virtual Sector Sector1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}
