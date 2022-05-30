using Quartz;
using Quartz.Impl;
using System;

namespace InsuranceAgency.Jobs
{
    public class MailSchedule
    {
        public static async void StartReportsSchedule()
        {
            //объект расписания
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ReportSender>().Build();

            // создаем триггер
            ITrigger trigger = TriggerBuilder.Create()
                                              .WithIdentity("trigger1", "group1") // идентифицируем триггер с именем и группой
                                              .WithCronSchedule("0 00 18 * * ?", x => x.InTimeZone(TimeZoneInfo.Local)) // каждый день в 18:00
                                              .Build(); // создаем триггер
            
            await scheduler.ScheduleJob(job, trigger); // начинаем выполнение работы
        }

        public static async void StartUsersSchedule()
        {
            //объект расписания
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<UserSender>().Build();

            // создаем триггер
            ITrigger trigger = TriggerBuilder.Create()
                                              .WithIdentity("trigger2", "group2") // идентифицируем триггер с именем и группой
                                              .WithCronSchedule("0 00 10 * * ?", x => x.InTimeZone(TimeZoneInfo.Local)) // каждый день в 9:30
                                              .Build(); // создаем триггер

            await scheduler.ScheduleJob(job, trigger); // начинаем выполнение работы
        }
    }
}