using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using Model.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers {
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase {

        EmpresaDbContext _db = new EmpresaDbContext();


        // GET api/values
        [HttpGet("GetList")]
        public ActionResult<object> GetList() {
            List<Clientes> Lista = _db.Clientes.ToList();
            return Lista;
        }
        [Authorize]
        [HttpGet("GetList2")]
        public ActionResult<object> GetList2() {
            List<Clientes> Lista = _db.Clientes.ToList();
            return Lista;
        }
    }
}
