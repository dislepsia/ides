using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl
{
    [MetadataType(typeof(nivelacionMetadata))]
    public partial class nivelacion : IAuditable
    {
    }

    public class nivelacionMetadata
    {
       
    }
}