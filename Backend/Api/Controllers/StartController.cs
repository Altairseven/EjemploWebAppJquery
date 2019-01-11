using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    public class StartController : ControllerBase {

        public ActionResult<string> Get() {
            return "OK";
        }





    }
}