using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Hosting;
using InsuranceAgency.Models;
using OfficeOpenXml;
using Quartz;

namespace InsuranceAgency.Jobs
{
    public class ReportSender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("agencyaadm@gmail.com", "AgencyAdmin");
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("agencyaadm@gmail.com", "ASP12345");
            smtp.EnableSsl = true;


            // кому отправляем
            MailAddress to = new MailAddress("agencyaadm@gmail.com");

            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Финансовый отчёт";
            // текст письма
            m.Body = "<h3>Финансовый отчёт</h3><p>Отчёт деятельности компании за последний день, месяц и год работы</p>";
            // письмо представляет код html
            m.IsBodyHtml = true;

            // вкладываем файл в письмо
            PrepareReport();
            string file_path = HostingEnvironment.MapPath("~/Content/Reports/ReportAllResult.xlsx");
            m.Attachments.Add(new Attachment(file_path));

            // отправляем асинхронно
            await smtp.SendMailAsync(m);
            m.Dispose();
        }

        private void PrepareReport()
        {
            // Путь к файлу с шаблоном
            string file_path_template = HostingEnvironment.MapPath("~/Content/Reports/ReportAll.xlsx");

            //Путь к файлу с результатом
            string file_path = HostingEnvironment.MapPath("~/Content/Reports/ReportAllResult.xlsx");

            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(file_path_template))
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


                // отчёт за день
                DateTime date = DateTime.Now.Date;
                int countPolicies1 = policies.Where(p => p.InsuranceType == "ОСАГО" && p.DateOfConclusion == date).Count();
                int sumInsurancePremium1 = policies.Where(p => p.InsuranceType == "ОСАГО" && p.DateOfConclusion == date).Sum(p => p.InsurancePremium);
                int sumInsurancePayment1 = insuranceEvents.Where(ie => ie.Policy.InsuranceType == "ОСАГО" && ie.Date == date).Sum(ie => ie.InsurancePayment);

                int countPolicies2 = policies.Where(p => p.InsuranceType == "КАСКО" && p.DateOfConclusion == date).Count();
                int sumInsurancePremium2 = policies.Where(p => p.InsuranceType == "КАСКО" && p.DateOfConclusion == date).Sum(p => p.InsurancePremium);
                int sumInsurancePayment2 = insuranceEvents.Where(ie => ie.Policy.InsuranceType == "КАСКО" && ie.Date == date).Sum(ie => ie.InsurancePayment);

                int line = 3;
                worksheet.Cells[line, 2].Value = countPolicies1;
                worksheet.Cells[line, 3].Value = sumInsurancePremium1;
                worksheet.Cells[line, 4].Value = sumInsurancePayment1;

                line = 4;
                worksheet.Cells[line, 2].Value = countPolicies2;
                worksheet.Cells[line, 3].Value = sumInsurancePremium2;
                worksheet.Cells[line, 4].Value = sumInsurancePayment2;

                line = 5;
                worksheet.Cells[line, 2].Value = countPolicies1 + countPolicies2;
                worksheet.Cells[line, 3].Value = sumInsurancePremium1 + sumInsurancePremium2;
                worksheet.Cells[line, 4].Value = sumInsurancePayment1 + sumInsurancePayment2;

                
                // отчёт за месяц
                DateTime startDate = DateTime.Now.AddMonths(-1).Date;
                DateTime endDate = DateTime.Now.Date;

                countPolicies1 = policies.Where(p => p.InsuranceType == "ОСАГО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Count();
                sumInsurancePremium1 = policies.Where(p => p.InsuranceType == "ОСАГО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Sum(p => p.InsurancePremium);
                sumInsurancePayment1 = insuranceEvents.Where(ie => ie.Policy.InsuranceType == "ОСАГО" && ie.Date >= startDate && ie.Date <= endDate).Sum(ie => ie.InsurancePayment);

                countPolicies2 = policies.Where(p => p.InsuranceType == "КАСКО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Count();
                sumInsurancePremium2 = policies.Where(p => p.InsuranceType == "КАСКО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Sum(p => p.InsurancePremium);
                sumInsurancePayment2 = insuranceEvents.Where(ie => ie.Policy.InsuranceType == "КАСКО" && ie.Date >= startDate && ie.Date <= endDate).Sum(ie => ie.InsurancePayment);

                line = 10;
                worksheet.Cells[line, 2].Value = countPolicies1;
                worksheet.Cells[line, 3].Value = sumInsurancePremium1;
                worksheet.Cells[line, 4].Value = sumInsurancePayment1;

                line = 11;
                worksheet.Cells[line, 2].Value = countPolicies2;
                worksheet.Cells[line, 3].Value = sumInsurancePremium2;
                worksheet.Cells[line, 4].Value = sumInsurancePayment2;

                line = 12;
                worksheet.Cells[line, 2].Value = countPolicies1 + countPolicies2;
                worksheet.Cells[line, 3].Value = sumInsurancePremium1 + sumInsurancePremium2;
                worksheet.Cells[line, 4].Value = sumInsurancePayment1 + sumInsurancePayment2;


                // отчёт за год
                startDate = DateTime.Now.AddYears(-1);
                endDate = DateTime.Now.Date;

                countPolicies1 = policies.Where(p => p.InsuranceType == "ОСАГО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Count();
                sumInsurancePremium1 = policies.Where(p => p.InsuranceType == "ОСАГО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Sum(p => p.InsurancePremium);
                sumInsurancePayment1 = insuranceEvents.Where(ie => ie.Policy.InsuranceType == "ОСАГО" && ie.Date >= startDate && ie.Date <= endDate).Sum(ie => ie.InsurancePayment);

                countPolicies2 = policies.Where(p => p.InsuranceType == "КАСКО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Count();
                sumInsurancePremium2 = policies.Where(p => p.InsuranceType == "КАСКО" && p.DateOfConclusion >= startDate && p.DateOfConclusion <= endDate).Sum(p => p.InsurancePremium);
                sumInsurancePayment2 = insuranceEvents.Where(ie => ie.Policy.InsuranceType == "КАСКО" && ie.Date >= startDate && ie.Date <= endDate).Sum(ie => ie.InsurancePayment);

                line = 17;
                worksheet.Cells[line, 2].Value = countPolicies1;
                worksheet.Cells[line, 3].Value = sumInsurancePremium1;
                worksheet.Cells[line, 4].Value = sumInsurancePayment1;

                line = 18;
                worksheet.Cells[line, 2].Value = countPolicies2;
                worksheet.Cells[line, 3].Value = sumInsurancePremium2;
                worksheet.Cells[line, 4].Value = sumInsurancePayment2;

                line = 19;
                worksheet.Cells[line, 2].Value = countPolicies1 + countPolicies2;
                worksheet.Cells[line, 3].Value = sumInsurancePremium1 + sumInsurancePremium2;
                worksheet.Cells[line, 4].Value = sumInsurancePayment1 + sumInsurancePayment2;

                //сохраняем в новое место
                excelPackage.SaveAs(file_path);
            }
        }
    }
}