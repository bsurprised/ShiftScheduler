using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Onyx.ShiftScheduler.Api.Filters;
using Onyx.ShiftScheduler.Core.Scheduler.Dto;
using Onyx.ShiftScheduler.Infrastructure.Services;

namespace Onyx.ShiftScheduler.Api.Controllers
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidateModel]
    public class SchedulesController : ControllerBase
    {
        private readonly SchedulerService _schedulerService;

        public SchedulesController(SchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
        }

        // POST /v1/schedules
        [HttpPost]
        public async Task<ScheduleDto> Get([FromBody] ScheduleRequestDto request)
        {
            // Generating a sample schedule for demo purposes
            return await _schedulerService.GetNewScheduleAsync(request);
        }
       
    }
}