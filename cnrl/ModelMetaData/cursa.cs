using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(cursaMetadata))]
    public partial class cursa : IAuditable
    {
    }

    public class cursaMetadata
    {
        [Required(ErrorMessage = "El campo Motivo Baja es obligatorio.")]
        public Nullable<int> baja { get; set; }

        [Required(ErrorMessage = "El campo Descripción Baja es obligatorio.")]
        public string DescripcionBaja { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor seleccione una oferta")]
        public int oferta { get; set; }
    }
}