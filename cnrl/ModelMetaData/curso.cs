using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(cursoMetadata))]
    public partial class curso : IAuditable
    {
    }

    public class cursoMetadata
    {

        [Required]
        public int codigo { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Required]
        public int horasSemanales { get; set; }
        [Required]
        public int concepto { get; set; }
        [Required]
        public int tipoCurso { get; set; }
        [Required]
        public int nivel { get; set; }
        [Required]
        public Nullable<int> codCurso { get; set; }

        /*[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> fechaDocumentacion { get; set; }*/
    }
}