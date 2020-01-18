using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(permisoMetadata))]
    public partial class permiso : IAuditable
    {
    }

    public class permisoMetadata
    {
       
    }
}