using Api.Reports;
using Api.Reports.TemplateService;
using Api.Repositories;
using DinkToPdf;
using DinkToPdf.Contracts;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcLanWebApi.Content.Reports {
    public class ClientesReport : Report {

        ClientesRepository _repository = new ClientesRepository();


        public override string ReportFileName {
            get { return "ClientesReport.cshtml"; }
        }
        public override string ReportCSSFileName {
            get { return "ListReportStyles.css"; }
        }
        public override string ReportTitle {
            get { return "Listado de Clientes"; }
        }

        public ClientesReport(IConverter conv, ITemplateService ts) : base(conv,ts){
            
        }

        public override ReportObject GetReport() {

            var List = _repository.GetList();

            var globalSettings = new GlobalSettings {
                ColorMode = ColorMode.Color,
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
                Margins = new MarginSettings { Top = 10, Left = 5, Right = 5 },
            };


            return new ReportObject{ Model = List, GlobalSettings = globalSettings };
        }
    }
}
