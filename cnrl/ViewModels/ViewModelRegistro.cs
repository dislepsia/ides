using cnrl.Models;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace cnrl.ViewModels
{
    public class ViewModelRegistro
    {
        public AspNetUsers usuario { get; set; }
        public CambioPasswordViewModel resetPassword { get; set; }


    }
}
