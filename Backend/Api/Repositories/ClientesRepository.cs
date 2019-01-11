using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.EntidadesExtendidas;

namespace Api.Repositories {
    public class ClientesRepository {


        EmpresaDbContext _db = new EmpresaDbContext();

        public List<ClientesEntity> GetList() {

            //Para mostrar en la grilla, necesitamos traer el cliente, pero tambien el nombre del tipo de documento, que esta en otra tabla

            List<ClientesEntity> lista = (from a in _db.Clientes
                                          join b in _db.Tipos_Documento on a.ID_TipoDocumento equals b.ID
                                          select new ClientesEntity {
                                              ID = a.ID,
                                              Nombre = a.Apellido + ", " + a.Nombre,
                                              Nombre_TipoDocumento = b.Nombre,
                                              Nro_Documento = a.Nro_Documento,
                                              Email = a.Email,
                                              Telefono = a.Telefono,
                                              //De paso tambien concatenamos toda la direccion junta, para mostrarla mas facil.
                                              Direccion = a.Dir_Calle.Trim() + " " + a.Dir_Numer.Trim() + " " + a.Dir_Piso.Trim() + " " + a.Dir_Dpto
                                          }).ToList();
            return lista;
        }

        public Clientes GetClienteById(long id) {
            return _db.Clientes.FirstOrDefault(x => x.ID == id);
        }

        public string Create(ClientesEntity Objeto) {
            
            string result = "";
            try {
                Clientes Entity = new Clientes();

                Entity.Nombre = Objeto.Nombre;
                Entity.Apellido = Objeto.Apellido;
                Entity.Email = Objeto.Email;
                Entity.ID_TipoDocumento = Objeto.ID_TipoDocumento;
                Entity.Nro_Documento = Objeto.Nro_Documento;
                Entity.Telefono = Objeto.Telefono;
                Entity.Dir_Calle = Objeto.Dir_Calle;
                Entity.Dir_Numer = Objeto.Dir_Numer;
                Entity.Dir_Piso = Objeto.Dir_Piso;
                Entity.Dir_Dpto = Objeto.Dir_Dpto;

                _db.Clientes.Add(Entity);

                _db.SaveChanges();

            }
            catch (Exception ex) {
                result = "Se produjo un error inesperado al realizar la operación";
                throw ex;
            }
            return result;
        }

        public string Update(ClientesEntity Objeto) {

            string result = "";
            try {
                Clientes Entity = _db.Clientes.FirstOrDefault(x => x.ID == Objeto.ID);
                if (Entity == null)
                    throw new Exception();

                Entity.Nombre = Objeto.Nombre;
                Entity.Apellido = Objeto.Apellido;
                Entity.Email = Objeto.Email;
                Entity.ID_TipoDocumento = Objeto.ID_TipoDocumento;
                Entity.Nro_Documento = Objeto.Nro_Documento;
                Entity.Telefono = Objeto.Telefono;
                Entity.Dir_Calle = Objeto.Dir_Calle;
                Entity.Dir_Numer = Objeto.Dir_Numer;
                Entity.Dir_Piso = Objeto.Dir_Piso;
                Entity.Dir_Dpto = Objeto.Dir_Dpto;

                _db.SaveChanges();

            }
            catch (Exception ex) {
                result = "Se produjo un error inesperado al realizar la operación";
                throw ex;
            }
            return result;
        }

        public string Delete(long id) {

            string result = "";
            try {
                Clientes Entity = _db.Clientes.FirstOrDefault(x => x.ID == id);
                if (Entity == null)
                    throw new Exception();

                _db.Clientes.Remove(Entity);

                _db.SaveChanges();

            }
            catch (Exception ex) {
                result = "Se produjo un error inesperado al realizar la operación";
                throw ex;
            }
            return result;
        }
    }
}
