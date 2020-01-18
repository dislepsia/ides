using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cnrl
{
    [MetadataType(typeof(AspNetUsersMetadata))]
    public partial class AspNetUsers : IAuditable
    {
    }

    public class AspNetUsersMetadata
    {
        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Formado de Email incorrecto")]
        public string Email;
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        [Required]
        public string Telefono { get; set; }
        [DisplayName("Celular")]
        public string Telefono2 { get; set; }

        //[Remote("IsUserExists", "Usuarios", ErrorMessage = "El Tipo y Número de Documento que ingresó ya existen")]
        //public string UserName { get; set; }

    }
}