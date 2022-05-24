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
                                              .WithCronSchedule("0 30 23 * * ?", x => x.InTimeZone(TimeZoneInfo.Local)) // каждый день в 23:30
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
                                              .WithCronSchedule("0 30 8 * * ?", x => x.InTimeZone(TimeZoneInfo.Local)) // каждый день в 8:30
                                              .Build(); // создаем триггер

            await scheduler.ScheduleJob(job, trigger); // начинаем выполнение работы
        }
    }
}