using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(tipocursoMetadata))]
    public partial class tipocurso : IAuditable
    {
    }

    public class tipocursoMetadata
    {
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public string descripcion { get; set; }
    }
}