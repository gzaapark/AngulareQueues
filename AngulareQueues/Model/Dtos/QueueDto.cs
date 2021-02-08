using AngulareQueues.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularQueue.Model.Dtos
{
    public class QueueDto
    {
        public int Id { get; set; }
        public TimeSpan ActiveStart { get; set; }
        public TimeSpan ActiveEnd { get; set; }
        public List<TaskDto> Tasks { get; set; }
        public List<TaskDto> FinishedTasks { get; set; }

        public void RemoveOldTasks(DateTime now)
        {
            if (FinishedTasks == null)
                FinishedTasks = new List<TaskDto>();

            var oldTasks = Tasks != null && Tasks.Count > 0 ? Tasks.Where(x => x.Date.AddMinutes(x.FactDuration) < now).ToList() : null;
            if (oldTasks != null)
            {
                foreach (var oldTask in oldTasks)
                {
                    FinishedTasks.Add(oldTask);
                    Tasks.Remove(oldTask);
                }
            }
        }

        public void RemoveAllTasks()
        {
            if(Tasks != null && Tasks.Count > 0)
                Tasks.Clear();

            if (FinishedTasks != null && FinishedTasks.Count > 0)
                FinishedTasks.Clear();
        }

        public DateTime GetAvailableDateForNewTask(DateTime now)
        {
            var todayStart = now.Date + ActiveStart;
            var todayEnd = now.Date + ActiveEnd;
            var allPlanDuration = (todayEnd - todayStart).TotalMinutes;

            if (now < todayEnd && now >= todayStart)
            {
                todayStart = new DateTime(Math.Max(todayStart.Ticks, now.Ticks));

                if (Tasks == null || Tasks.Count == 0)
                    return todayStart;

                var allFactDuration = 0; // Minutes
                foreach (var task in Tasks)
                {
                    var planDuration = task.TaskType.Duration;
                    var factDuration = task.FactDuration;
                    var date = task.Date;

                    if (date.AddMinutes(factDuration) < now)
                        allFactDuration += factDuration;
                    else
                        allFactDuration += planDuration;
                }

                if (allFactDuration < allPlanDuration)
                    return todayStart.AddMinutes(allFactDuration);
                else
                    throw new AvailableTimeException();
            }
            else
                throw new OfficeHoursException();
        }
        public void AddTask(TaskDto task)
        {
            if (Tasks == null)
                Tasks = new List<TaskDto>();
            Tasks.Add(task);
        }
    }
}
