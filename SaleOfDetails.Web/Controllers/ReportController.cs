using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoMapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SaleOfDetails.Domain.DataAccess.Interfaces;
using SaleOfDetails.Domain.Models;
using SaleOfDetails.Web.Models;

namespace SaleOfDetails.Web.Controllers
{
    public class ReportController : BaseController
    {
        public ReportController(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {

        }

        public FileContentResult SalesReport()
        {
            var fileDownloadName = String.Format("SalesReport.xlsx");
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            var sales = UnitOfWork.Repository<Sale>()
                .Get(orderBy: o => o.OrderBy(x => x.SaleDate),
                    includeProperties: "Product, Employee, Employee.Person, Client, Client.Person")
                .ToList();

            var saleViewModels = Mapper.Map<List<Sale>, List<SaleViewModel>>(sales);
            ExcelPackage package = GenerateExcelFile(saleViewModels);

            var fsr = new FileContentResult(package.GetAsByteArray(), contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        private static ExcelPackage GenerateExcelFile(List<SaleViewModel> datasource)
        {
            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");

            ws.Cells[1, 1].Value = "№";
            ws.Cells[1, 2].Value = "Название детали / спецтехники";
            ws.Cells[1, 3].Value = "Количество";
            ws.Cells[1, 4].Value = "Общая стоимость";
            ws.Cells[1, 5].Value = "ФИО покупателя";
            ws.Cells[1, 6].Value = "ФИО продавца";
            ws.Cells[1, 7].Value = "Дата продажи";

            for (int i = 0; i < datasource.Count(); i++)
            {
                var obj = datasource.ElementAt(i);

                ws.Cells[i + 2, 1].Value = i + 1;
                ws.Cells[i + 2, 2].Value = obj.ProductName;
                ws.Cells[i + 2, 3].Value = obj.NumberOfProducts;
                ws.Cells[i + 2, 4].Value = obj.NumberOfProducts * obj.ProductCost;
                ws.Cells[i + 2, 5].Value = obj.ClientFullName;
                ws.Cells[i + 2, 6].Value = obj.EmployeeFullName;
                ws.Cells[i + 2, 7].Value = obj.SaleDate?.ToString("dd.MM.yyyy") ?? "";
            }

            using (ExcelRange rng = ws.Cells["A1:G1"])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; 
                rng.Style.Fill.BackgroundColor.SetColor(Color.Gold); 
                rng.Style.Font.Color.SetColor(Color.Black);
            }
            return pck;
        }
    }
}
