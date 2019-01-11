using Api.Reports.TemplateService;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Reports {
    public class Report {

        private IConverter _converter;
        private ITemplateService _templateService;


        public virtual string ReportFileName { get; }
        public virtual string ReportCSSFileName { get; }
        public virtual string ReportTitle { get; }

        public Report(IConverter converter, ITemplateService templateService) {
            _converter = converter;
            _templateService = templateService;
        }

        public virtual ReportObject GetReport() {
            return new ReportObject();
        }


        public async Task<byte[]> Export() {

            ReportObject R = this.GetReport();

            string View = this.ReportFileName;
            string css = this.ReportCSSFileName;

            R.GlobalSettings.DocumentTitle = this.ReportTitle;

            R.ObjectSettings.HtmlContent = await _templateService.RenderTemplateAsync(View, R.Model);

            R.ObjectSettings.WebSettings.DefaultEncoding = "utf-8";
            R.ObjectSettings.WebSettings.UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Content/CSS", css);

            //R.ObjectSettings.PagesCount = true;

            //R.ObjectSettings.HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //R.ObjectSettings.FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }


            var pdf = new HtmlToPdfDocument() {
                GlobalSettings = R.GlobalSettings,
                Objects = { R.ObjectSettings }
            };

            return _converter.Convert(pdf);

        }
    }

    public class ReportObject {
        public dynamic Model { get; set; }
        public GlobalSettings GlobalSettings { get; set; }
        public ObjectSettings ObjectSettings { get; set; }

        public ReportObject() {
            Model = null;
            GlobalSettings = new GlobalSettings {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Left= 10, Right= 10 },
                DocumentTitle = "ArcLan Report"
            };
            ObjectSettings = new ObjectSettings();
        }

        public ReportObject(object _model, GlobalSettings _globalSettings, ObjectSettings _objectSettings) {
            this.Model = _model;
            this.GlobalSettings = _globalSettings;

            if(GlobalSettings == null)
                GlobalSettings = new GlobalSettings {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "ArcLan Report"
                };

            this.ObjectSettings = _objectSettings;
            if(ObjectSettings == null)
                ObjectSettings = new ObjectSettings();
        }
    }


}
