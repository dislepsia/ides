using System.ComponentModel.DataAnnotations;

namespace cnrl.Models
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Dni { get; set; }
        public int Jurisdiccion { get; set; }
        public int Red { get; set; }
        public string Rol { get; set; }
        public string JurisdiccionNombre { get; set; }
        public string RedNombre { get; set; }
        public string RolNombre { get; set; }
        public string PasswordInicial { get; set; }

        public bool Activo { get; set; }
        
    }
}
