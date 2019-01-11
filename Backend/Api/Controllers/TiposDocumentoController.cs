using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;

namespace Api.Controllers {
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    public class TiposDocumentoController : ControllerBase {

        TiposDocumentoRepository _repository = new TiposDocumentoRepository();

        [HttpGet("GetList")]
        public ActionResult<object> GetList() {
            return _repository.GetList();
        }



    }
}