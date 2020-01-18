using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cnrl.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = @"Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = @"Recordar este equipo?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = @"Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = @"Tipo Documento")]
        public int TipoDocumento { get; set; }

        [Required]
        [Display(Name = @"Nro Documento")]
        //[EmailAddress]
        public long NroDocumento { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = @"Contraseña")]
        public string Password { get; set; }

        [Display(Name = @"No cerrar seseión")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = @"Email")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = @"La {0} debe tener al menos {2} caracteres.", MinimumLength = 4)]
        //[DataType(DataType.Password)]
        //[Display(Name = @"Contraseña")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = @"Confirmar Contraseña")]
        //[Compare("Password", ErrorMessage = @"Las Contraseñas no coinciden.")]
        //public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = @"Tipo Documento")]
        public int TipoDocumento { get; set; }

        [Required]
        [Display(Name = @"Nro Documento")]
        //[EmailAddress]
        public long NroDocumento { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = @"La {0} debe tener al menos {2} caracteres.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = @"Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = @"Las Contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class CambioPasswordViewModel
    {
        
        [Required]
        [StringLength(100, ErrorMessage = @"La {0} debe tener al menos {2} caracteres.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = @"Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = @"Las Contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

    }
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = @"Tipo Documento")]
        public int TipoDocumento { get; set; }

        [Required]
        [Display(Name = @"Nro Documento")]
        //[EmailAddress]
        public long NroDocumento { get; set; }
    }
}
