using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(precioMetadata))]
    public partial class precio : IAuditable
    {
    }

    public class precioMetadata
    {

        //[DisplayName("Alumno ($)")]
        //[Required(ErrorMessage = "Debe de ingresar un arancel para Alumno.")]
        //[Range(typeof(Double), "0", "9999", ErrorMessage = "El valor {0} debe ser numérico.")]

        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "El arancel debe contener sólo números.")]
        //[Range(1, 100, ErrorMessage = "El valor {0} debe ser numérico.")]

        [Required(ErrorMessage = "El campo Alumno es Obligatorio")]

        //[Range(0.01, 999999999, ErrorMessage = "El arancel debe ser mayor a 0.00")]

        public double alumno { get; set; }

        //[Required(ErrorMessage = "El campo No Alumno es Obligatorio")]
        //[Range(0.01, 999999999, ErrorMessage = "El arancel debe ser mayor a 0.00")]
        public double noalumno { get; set; }

        //[Required(ErrorMessage = "El campo Empleado es Obligatorio")]
        //[Range(0.01, 999999999, ErrorMessage = "El arancel debe ser mayor a 0.00")]
        public double empleado { get; set; }

        //[Required(ErrorMessage = "El campo Mayor de 60 es Obligatorio")]
        //[Range(0.01, 999999999, ErrorMessage = "El arancel debe ser mayor a 0.00")]
        public double mayorDe60 { get; set; }


    }
}