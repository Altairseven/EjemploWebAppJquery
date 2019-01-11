using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.EntidadesExtendidas;

namespace Api.Repositories {
    public class TiposDocumentoRepository {


        EmpresaDbContext _db = new EmpresaDbContext();

        public List<SelectEntity> GetList() {
            List<SelectEntity> Lista = _db.Tipos_Documento.Select(x => new SelectEntity {
                ID = x.ID,
                Nombre = x.Nombre
            }).ToList();

            return Lista;
        }
    }
}
