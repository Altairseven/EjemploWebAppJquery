using System;
using System.Collections.Generic;

namespace Model.Entities
{
    public partial class Clientes
    {
        public long ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public long ID_TipoDocumento { get; set; }
        public string Nro_Documento { get; set; }
        public string Telefono { get; set; }
        public string Dir_Calle { get; set; }
        public string Dir_Numer { get; set; }
        public string Dir_Piso { get; set; }
        public string Dir_Dpto { get; set; }
    }
}
