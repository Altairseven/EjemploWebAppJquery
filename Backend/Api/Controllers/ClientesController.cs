using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Configuration;
using Api.EntidadesExtendidas;
using Api.Reports.TemplateService;
using Api.Repositories;
using ArcLanWebApi.Content.Reports;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Model.Entities;

namespace Api.Controllers {
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    public class ClientesController : ControllerBase {

        ClientesRepository _repository = new ClientesRepository();

        //Necesario para Exportaciones
        private IConverter _converter;
        private ITemplateService _templateService;
        public ClientesController(IConverter converter, IRazorViewEngine engine, IServiceProvider service, ITempDataProvider provider) {
            this._converter = converter;
            this._templateService = new TemplateService(engine, service, provider);
        }



        [HttpGet("GetList")]
        public ActionResult<object> GetList() {
            return _repository.GetList();
        }
        [HttpGet("GetClienteById")]
        public ActionResult<object> GetClienteById([FromQuery] long id) {

            return _repository.GetClienteById(id);
        }

        [HttpPost("Create")]
        public ActionResult<object> Create([FromBody] ClientesEntity Objeto) {
            string result = _repository.Create(Objeto);
            if (result != "")
                return BadRequest(result);
            else
                return Ok();
        }

        [HttpPost("Update")]
        public ActionResult<object> Update([FromBody] ClientesEntity Objeto) {
            string result = _repository.Update(Objeto);
            if (result != "")
                return BadRequest(result);
            else
                return Ok();
        }

        [HttpGet("Delete")]
        public ActionResult<object> Delete([FromQuery] long id) {
            string result = _repository.Delete(id);
            if (result != "")
                return BadRequest(result);
            else
                return Ok();
        }

        [HttpGet("ExportPdf")]
        [AllowAnonymous]
        public async Task<FileResult> ExportPdf() {

            ClientesReport Report = new ClientesReport(this._converter, this._templateService);

            byte[] stream = await Report.Export();

            Request.HttpContext.Response.Headers.Add("filename", "UsuariosReport2.pdf");
            return File(stream, "application/pdf", "UsuariosReport2.pdf");
        }

        [HttpGet("ExportExcel")]
        [AllowAnonymous]
        public FileResult ExportExcel() {

            ExcelReport excel = new ExcelReport();

            List<ClientesEntity> List = _repository.GetList();

            var columns = new Dictionary<string, ExcelColumn>();
            columns.Add("ID", new ExcelColumn { Name = "ID", Type = "number", Width = 1000 });
            columns.Add("Nombre", new ExcelColumn { Name = "Nombre", Type = "text", Width = 4000 });
            columns.Add("Apellido", new ExcelColumn { Name = "Apellido", Type = "text", Width = 4000 });
            columns.Add("Nro_Documento", new ExcelColumn { Name = "Documento", Type = "text", Width = 5000 });
          
            byte[] bytes = excel.GetExcel<ClientesEntity>(columns, List);

            Request.HttpContext.Response.Headers.Add("filename", "ExcelTest.xlsx");
            return File(bytes, "application/vnd.ms-excel", "ExcelTest.xlsx");


        }

    }
}