using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(sedeMetadata))]
    public partial class sede : IAuditable
    {
    }

    public class sedeMetadata
    {
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} elementos.")]
        public string nombre { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessage = "El {0} debe contener sólo números.")]
        public string telefono { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Formado de Email incorrecto.")]
        public string mail { get; set; }
    }
}