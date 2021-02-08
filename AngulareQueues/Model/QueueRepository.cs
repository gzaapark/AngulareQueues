using AngulareQueues;
using AngulareQueues.Model.Exceptions;
using AngularQueue.Model.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AngularQueue.Model
{
    public class QueueRepository
    {
        private static readonly SemaphoreSlim Locker = new SemaphoreSlim(1, 1);

        private readonly CacheWrapper _cache;
        private readonly IOptions<AppSettings> _appSettings;

        public QueueRepository(IMemoryCache cache, IOptions<AppSettings> appSettings)
        {
            _cache = new CacheWrapper(cache);
            _appSettings = appSettings;
        }

        private List<QueueDto> getActualQueues()
        {
            var queueList = _cache.GetQueueList();

            if (queueList == null)
            {
                queueList = _appSettings.Value.Queues;
                _cache.SetQueueList(queueList);
            }

            return queueList;
        }

        public List<TaskDto> GenerateRandomTasks(DateTime now)
        {
            var rnd = new Random();
            var tasks = new List<TaskDto>();
            var queues = getActualQueues();
            var taskTypes = GetTaskTypes();

            double allDuration = 0;
            foreach(var queue in queues)
            {
                var activeStart = now.Date + queue.ActiveStart;
                var activeEnd = now.Date + queue.ActiveEnd;

                if(now >= activeStart && now < activeEnd)
                {
                    activeStart = new DateTime(Math.Max(activeStart.Ticks, now.Ticks));
                    allDuration += (activeEnd - activeStart).TotalMinutes;
                }
            }

            if (allDuration == 0)
                throw new Exception("Текущая дата должна попадать в промежуток [ActiveStart, ActiveEnd) хотя бы для одной из очередей (см. appsettings.json)");

            double factAllDuration = 0;
            do
            {
                var taskType = taskTypes[rnd.Next(0, taskTypes.Count - 1)];
                var durationDifference = rnd.Next(taskTypes.Min(x => x.Duration), taskTypes.Max(x => x.Duration));
                var factDuration = taskType.Duration + durationDifference;

                tasks.Add(new TaskDto
                {
                    FactDuration = factDuration,
                    TaskType = taskType
                });

                factAllDuration += factDuration;

            } while (factAllDuration <= allDuration);

            return tasks;
        }
        public async Task<TaskDto> AddTaskToAnyQueue(TaskDto task, DateTime now)
        {
            await Locker.WaitAsync();
            try
            {
                var queues = getActualQueues();
                foreach (var queue in queues)
                    queue.RemoveOldTasks(now);

                var availableDate = DateTime.MaxValue;
                QueueDto availableQueue = null;

                foreach (var queue in queues)
                {
                    try
                    {
                        var queueAvailableDate = queue.GetAvailableDateForNewTask(now);
                        if (queueAvailableDate < availableDate)
                        {
                            availableDate = queueAvailableDate;
                            availableQueue = queue;
                        }
                    }
                    catch(AvailableTimeException)          
                    {
                    }
                    catch (OfficeHoursException)
                    {
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }

                if (availableDate == DateTime.MaxValue)
                    throw new Exception("Номерок выдать невозможно");

                var availableTaskId = queues.Max(x => x.Tasks != null && x.Tasks.Count > 0 ? x.Tasks.Max(y => y.Id) : 0) + 1;

                task.Id = availableTaskId;
                task.Date = availableDate;
                task.FactDuration = task.FactDuration > 0 ? task.FactDuration : task.TaskType.Duration;
                task.QueueId = availableQueue.Id;

                availableQueue.AddTask(task);

                _cache.SetQueueList(queues);

                return task;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Locker.Release();
            }
        }
        public List<TaskTypeDto> GetTaskTypes()
        {
            var taskTypes = _cache.GetTaskTypeList();

            if(taskTypes == null)
            {
                taskTypes = _appSettings.Value.TaskTypes;
                _cache.SetTaskTypeList(taskTypes);
            }

            return taskTypes;
        }
        public async Task<List<QueueDto>> GetQueues(DateTime now)
        {
            await Locker.WaitAsync();
            try
            {
                var queues = getActualQueues();
                foreach (var queue in queues)
                    queue.RemoveOldTasks(now);

                _cache.SetQueueList(queues);

                return queues;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Locker.Release();
            }
        }
        public async Task ClearAllTasks()
        {
            await Locker.WaitAsync();
            try
            {
                var queues = getActualQueues();
                foreach (var queue in queues)
                    queue.RemoveAllTasks();

                _cache.SetQueueList(queues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Locker.Release();
            }
        }
    }
}
