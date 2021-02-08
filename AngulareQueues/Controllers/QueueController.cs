using AngularQueue.Model;
using AngularQueue.Model.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngulareQueues.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly QueueRepository _repository;
        public QueueController(IMemoryCache cache, IOptions<AppSettings> appSettings)
        {
            _repository = new QueueRepository(cache, appSettings);
        }

        [HttpGet("[action]")]
        public ActionResult<List<TaskTypeDto>> GetTaskTypes()
        {
            try
            {
                return Ok(_repository.GetTaskTypes());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<QueueDto>>> GetQueues()
        {
            try
            {
                var now = DateTime.Parse(HttpContext.Request.Query["now"].ToString());
                var queues = await _repository.GetQueues(now);
                return Ok(queues);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TaskDto>> AddTaskToAnyQueue([FromBody] TaskDto task)
        {
            try
            {
                var now = DateTime.Parse(HttpContext.Request.Query["now"].ToString());
                task = await _repository.AddTaskToAnyQueue(task, now);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<List<TaskDto>> GenerateRandomTasks()
        {
            try
            {
                var now = DateTime.Parse(HttpContext.Request.Query["now"].ToString());
                var randomTasks = _repository.GenerateRandomTasks(now);
                return Ok(randomTasks);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ClearAllTasks()
        {
            try
            {
                await _repository.ClearAllTasks();
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
