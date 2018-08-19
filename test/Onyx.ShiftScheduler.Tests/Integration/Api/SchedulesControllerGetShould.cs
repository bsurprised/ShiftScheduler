using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Onyx.ShiftScheduler.Api;
using Onyx.ShiftScheduler.Core.Helpers;
using Onyx.ShiftScheduler.Core.Scheduler.Dto;
using Xunit;

namespace Onyx.ShiftScheduler.Tests.Integration.Api
{

    public class SchedulesControllerGetShould : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public SchedulesControllerGetShould(TestWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReturnsDemoSchedule()
        {
            var response = await _client.GetAsync("/v1/schedules");
            response.EnsureSuccessStatusCode();

            var webResponse = await response.Content.ReadAsStringAsync();
            var schedules = JsonConvert.DeserializeObject<ScheduleDto>(webResponse);

            Assert.NotNull(schedules.Name);
            Assert.NotNull(schedules.Statistics);

            var startDate = DateTime.SpecifyKind(Utils.GetNextWeekday(DateTime.Now, DayOfWeek.Monday), DateTimeKind.Unspecified);

            Assert.Equal(startDate.Date, schedules.StartDate);
            Assert.Equal(startDate.AddDays(14).Date, schedules.EndDate);

            Assert.Equal(140, schedules.Shifts.Count());
            foreach (var shift in schedules.Shifts)
            {
                Assert.NotNull(shift.Employee);
                //...
            }
        }
    }
}
