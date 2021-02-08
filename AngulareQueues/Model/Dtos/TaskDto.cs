using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularQueue.Model.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int FactDuration { get; set; }
        public TaskTypeDto TaskType { get; set; }
        public int QueueId { get; set; }
        public DateTime EndDate
        {
            get
            {
                return Date.AddMinutes(FactDuration);
            }
        }
    }
}
