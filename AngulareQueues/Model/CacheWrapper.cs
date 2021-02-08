using AngularQueue.Model.Dtos;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularQueue.Model
{
    public class CacheWrapper
    {
        private readonly string TASK_TYPE_LIST = "TASK_TYPE_LIST";
        private readonly string QUEUE_LIST = "QUEUE_LIST";

        private readonly MemoryCacheEntryOptions _entryOptions;
        private readonly IMemoryCache _cache;
        public CacheWrapper(IMemoryCache cache)
        {
            _cache = cache;
            _entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }


        public List<TaskTypeDto> GetTaskTypeList()
        {
            return _cache.Get(TASK_TYPE_LIST) as List<TaskTypeDto>;
        }
        public void SetTaskTypeList(List<TaskTypeDto> value)
        {
            _cache.Set(TASK_TYPE_LIST, value, _entryOptions);
        }


        public List<QueueDto> GetQueueList()
        {
            return _cache.Get(QUEUE_LIST) as List<QueueDto>;
        }
        public void SetQueueList(List<QueueDto> value)
        {
            _cache.Set(QUEUE_LIST, value, _entryOptions);
        }


    }
}
