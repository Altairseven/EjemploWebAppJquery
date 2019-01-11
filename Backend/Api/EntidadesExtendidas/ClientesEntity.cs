using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Entities;

namespace Api.EntidadesExtendidas {
    public class ClientesEntity : Clientes {

        public string Nombre_TipoDocumento { get; set; }
        public string Direccion { get; set; }


        public ClientesEntity() {

        }

        public ClientesEntity(Clientes x) {
            
        }
    }
}
