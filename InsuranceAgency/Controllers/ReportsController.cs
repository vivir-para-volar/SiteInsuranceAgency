using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using InsuranceAgency.Models.Security;
using OfficeOpenXml;
using Microsoft.AspNet.Identity.EntityFramework;
using InsuranceAgency.Models;
using InsuranceAgency.Models.ViewModels;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            ViewBag.InsuranceType = new SelectList(new List<string> { "ОСАГО и КАСКО", "ОСАГО", "КАСКО" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult GetUsersReport()
        {
            // Путь к файлу с шаблоном
            string file_path_template = Server.MapPath("~/Content/Reports/UsersReport.xlsx");
            FileInfo fi = new FileInfo(file_path_template);

            //Путь к файлу с результатом
            string file_path = Server.MapPath("~/Content/Reports/UsersReportResult.xlsx");
            FileInfo fi_report = new FileInfo(file_path);

            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Лямина И.А.";
                excelPackage.Workbook.Properties.Title = "Список пользователей системы";
                excelPackage.Workbook.Properties.Subject = "Пользователи системы";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //получаем лист по имени
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Users"];


                var db = new MyIdentityDbContext();

                var userStore = new UserStore<MyIdentityUser>(db);
                var userManager = new UserManager<MyIdentityUser>(userStore);

                var roleStore = new RoleStore<MyIdentityRole>(db);
                var roleManager = new RoleManager<MyIdentityRole>(roleStore);

                //получаем списко пользователей и в цикле заполняем лист данными
                List<MyIdentityUser> users = userManager.Users.ToList();

                int startLine = 3;
                foreach (var user in users)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = user.UserName;
                    worksheet.Cells[startLine, 3].Value = user.FullName;
                    worksheet.Cells[startLine, 4].Value = user.BirthDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[startLine, 5].Value = user.PhoneNumber;
                    worksheet.Cells[startLine, 6].Value = user.Email;

                    var roles = user.Roles.ToArray();
                    string role = roleManager.Roles.ToArray().First(r => r.Id == roles[0].RoleId).Name;

                    switch (role)
                    {
                        case "Administrator":
                            worksheet.Cells[startLine, 7].Value = "Администратор";
                            break;
                        case "Operator":
                            worksheet.Cells[startLine, 7].Value = "Оператор";
                            break;
                        case "User":
                            worksheet.Cells[startLine, 7].Value = "Пользователь";
                            break;
                    }

                    startLine++;
                }

                //созраняем в новое место
                excelPackage.SaveAs(fi_report);
            }

            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "UsersReport.xlsx";

            return File(file_path, file_type, file_name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult GetPoliciesReport()
        {
            // Путь к файлу с шаблоном
            string file_path_template = Server.MapPath("~/Content/Reports/PoliciesReport.xlsx");
            FileInfo fi = new FileInfo(file_path_template);

            //Путь к файлу с результатом
            string file_path = Server.MapPath("~/Content/Reports/PoliciesReportResult.xlsx");
            FileInfo fi_report = new FileInfo(file_path);

            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Лямина И.А.";
                excelPackage.Workbook.Properties.Title = "Список полисов";
                excelPackage.Workbook.Properties.Subject = "Полисы";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //получаем лист по имени
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Policies"];

                //получаем списко полисов и в цикле заполняем лист данными
                AgencyDBContext db = new AgencyDBContext();
                List<Policy> policies = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder).ToList();

                int startLine = 3;
                foreach (var policy in policies)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = policy.InsuranceType;
                    worksheet.Cells[startLine, 3].Value = policy.InsurancePremium;
                    worksheet.Cells[startLine, 4].Value = policy.InsuranceAmount;
                    worksheet.Cells[startLine, 5].Value = policy.DateOfConclusion.ToString("dd-MM-yyyy");
                    worksheet.Cells[startLine, 6].Value = policy.ExpirationDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[startLine, 7].Value = policy.Policyholder.FullName;
                    worksheet.Cells[startLine, 8].Value = policy.Car.Model;
                    worksheet.Cells[startLine, 9].Value = policy.Employee.FullName;

                    startLine++;
                }

                //созраняем в новое место
                excelPackage.SaveAs(fi_report);
            }

            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "PoliciesReport.xlsx";

            return File(file_path, file_type, file_name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetReport([Bind(Include = "InsuranceType,StartDate,EndDate")] Report report)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.InsuranceType = new SelectList(new List<string> { "ОСАГО и КАСКО", "ОСАГО", "КАСКО" });
                return View("Index", report);
            }
            if(report.StartDate >= report.EndDate)
            {
                ModelState.AddModelError("StartDate", "Дата начала не может быть больше Даты окончания");
                ViewBag.InsuranceType = new SelectList(new List<string> { "ОСАГО и КАСКО", "ОСАГО", "КАСКО" });
                return View("Index", report);
            }

            // Путь к файлу с шаблоном
            string file_path_template = Server.MapPath("~/Content/Reports/Report.xlsx");
            FileInfo fi = new FileInfo(file_path_template);

            //Путь к файлу с результатом
            string file_path = Server.MapPath("~/Content/Reports/ReportResult.xlsx");
            FileInfo fi_report = new FileInfo(file_path);

            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Лямина И.А.";
                excelPackage.Workbook.Properties.Title = "Финансовый отчёт";
                excelPackage.Workbook.Properties.Subject = "Финансовый отчёт";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //получаем лист по имени
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Report"];

                //получаем списко полисов и заполняем лист данными
                AgencyDBContext db = new AgencyDBContext();
                List<Policy> policies = db.Policies.ToList();
                List<InsuranceEvent> insuranceEvents = db.InsuranceEvents.Include(ie => ie.Policy).ToList();

                int startLine = 3;
                if (report.InsuranceType == "ОСАГО и КАСКО")
                {
                    int countPolicies = policies.Where(p => p.DateOfConclusion >= report.StartDate && p.DateOfConclusion <= report.EndDate).Count();
                    int sumInsurancePremium = policies.Where(p => p.DateOfConclusion >= report.StartDate && p.DateOfConclusion <= report.EndDate).Sum(p => p.InsurancePremium);
                    int sumInsurancePayment = insuranceEvents.Where(ie => ie.Date >= report.StartDate && ie.Date <= report.EndDate).Sum(ie => ie.InsurancePayment);

                    worksheet.Cells[startLine, 1].Value = countPolicies;
                    worksheet.Cells[startLine, 2].Value = sumInsurancePremium;
                    worksheet.Cells[startLine, 3].Value = sumInsurancePayment;
                }
                else
                {
                    int countPolicies = policies.Where(p => p.InsuranceType == report.InsuranceType && p.DateOfConclusion >= report.StartDate && p.DateOfConclusion <= report.EndDate).Count();
                    int sumInsurancePremium = policies.Where(p => p.InsuranceType == report.InsuranceType && p.DateOfConclusion >= report.StartDate && p.DateOfConclusion <= report.EndDate).Sum(p => p.InsurancePremium);
                    int sumInsurancePayment = insuranceEvents.Where(ie => ie.Policy.InsuranceType == report.InsuranceType && ie.Date >= report.StartDate && ie.Date <= report.EndDate).Sum(ie => ie.InsurancePayment);

                    worksheet.Cells[startLine, 1].Value = countPolicies;
                    worksheet.Cells[startLine, 2].Value = sumInsurancePremium;
                    worksheet.Cells[startLine, 3].Value = sumInsurancePayment;
                }

                //созраняем в новое место
                excelPackage.SaveAs(fi_report);
            }

            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "Report.xlsx";

            return File(file_path, file_type, file_name);
        }
    }
}