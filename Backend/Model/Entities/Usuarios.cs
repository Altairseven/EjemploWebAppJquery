using System;
using System.Collections.Generic;

namespace Model.Entities
{
    public partial class Usuarios
    {
        public long ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaAlta { get; set; }
    }
}
