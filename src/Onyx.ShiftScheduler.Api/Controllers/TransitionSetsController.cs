using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Onyx.ShiftScheduler.Api.Filters;
using Onyx.ShiftScheduler.Core.App.Dto;
using Onyx.ShiftScheduler.Infrastructure.Services;

namespace Onyx.ShiftScheduler.Api.Controllers
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidateModel]
    public class TransitionSetsController : ControllerBase
    {
        private readonly SchedulerService _schedulerService;

        public TransitionSetsController(SchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
        }

        // GET /v1/transitionssets
        [HttpGet]
        public async Task<List<TransitionSetDto>> GetActiveTransitionSets()
        {
            // Getting saved and active transition sets
            return await _schedulerService.GetActiveTransitionSetsAsync();
        }

    }
}