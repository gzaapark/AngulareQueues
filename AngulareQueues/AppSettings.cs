using AngularQueue.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngulareQueues
{
    public class AppSettings
    {
        public List<TaskTypeDto> TaskTypes { get; set; }
        public List<QueueDto> Queues { get; set; }
    }
}
