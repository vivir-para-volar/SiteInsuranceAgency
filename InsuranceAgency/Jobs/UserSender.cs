using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using InsuranceAgency.Models;
using Quartz;

namespace InsuranceAgency.Jobs
{
    public class UserSender : IJob
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

            List<Policy> policies = GetListPolicy();
            foreach (Policy policy in policies)
            {
                // кому отправляем
                MailAddress to = new MailAddress(policy.Policyholder.Email);

                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Страховой полис";
                // текст письма
                m.Body = "<h3>Истечение срока действия полиса</h3><p>Срок действия вашего страхового полиса " + policy.InsuranceType + ", оформленного на автомобиль " + policy.Car.Model + " истекает сегодня</p>";
                // письмо представляет код html
                m.IsBodyHtml = true;

                // отправляем асинхронно
                await smtp.SendMailAsync(m);
                m.Dispose();
            }
        }

        private List<Policy> GetListPolicy()
        {
            // получаем список полисов, срок окончания которых выходит сегодня и в которых у страхователя есть email
            AgencyDBContext db = new AgencyDBContext();
            DateTime date = DateTime.Now.Date;
            List<Policy> policies = db.Policies.Include(p => p.Car)
                                               .Include(p => p.Policyholder)
                                               .Where(p => p.Policyholder.Email != null)
                                               .Where(p => p.ExpirationDate == date)
                                               .ToList();
            return policies;
        }
    }
}