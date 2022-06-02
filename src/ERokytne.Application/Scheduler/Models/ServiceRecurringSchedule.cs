using MassTransit.Scheduling;

namespace ERokytne.Application.Scheduler.Models
{
    public class ServiceRecurringSchedule : RecurringSchedule
    {
        public string TimeZoneId { get; set; }
        
        public DateTimeOffset StartTime { get; set; }
        
        public DateTimeOffset? EndTime { get; set; }
        
        public string ScheduleId { get; }
        
        public string ScheduleGroup { get; }
        
        public string CronExpression { get; }
        
        public string Description { get; set; }
        
        public MissedEventPolicy MisfirePolicy { get; set; }
        
        public ServiceRecurringSchedule(string scheduleId, string scheduleGroup, string cronExpression)
        {
            ScheduleId = scheduleId;
            ScheduleGroup = scheduleGroup;
            CronExpression = cronExpression;
        }
    }
}