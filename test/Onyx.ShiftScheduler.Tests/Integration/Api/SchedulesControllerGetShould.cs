using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            var startDate = DateTime.SpecifyKind(Utils.GetNextWeekday(DateTime.Now, DayOfWeek.Monday), DateTimeKind.Unspecified);

            var request = new ScheduleRequestDto()
            {
                TransitionSetId = 1,
                StartDate = startDate,
                Days = 14,
                MinShiftsPerCycle = 2,
                NumberOfEmployees = 10,
                ShiftHours = 12,
                StartHour = 7,
                TeamSize = 2,                
            };
            var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)));
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PostAsync("/v1/schedules", byteContent);
            response.EnsureSuccessStatusCode();

            var webResponse = await response.Content.ReadAsStringAsync();
            var schedules = JsonConvert.DeserializeObject<ScheduleDto>(webResponse);

            Assert.NotNull(schedules.Name);
            Assert.NotNull(schedules.Statistics);

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
