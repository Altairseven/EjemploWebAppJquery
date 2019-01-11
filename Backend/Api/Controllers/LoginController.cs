using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Repositories;
using Model.Entities;

namespace Api.Controllers {
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("SiteCorsPolicy")]
    public class LoginController : ControllerBase {

        LoginRepository _repository = new LoginRepository();

        [AllowAnonymous]
        [HttpPost("RequestToken")]
        public IActionResult RequestToken([FromBody] UserCredentials userinfo) {


            Usuarios Entity = _repository.CheckCredentials(userinfo.Login, userinfo.Password);

            if (Entity == null)
                return new UnauthorizedResult();

            _repository.UpdateUserlastLogin(Entity.ID);


            if (!string.IsNullOrEmpty(Entity.Username)) {
                return Ok(_repository.GenerateToken(Entity));
            }

            return BadRequest("Could not verify username and password");
        }
    }
}