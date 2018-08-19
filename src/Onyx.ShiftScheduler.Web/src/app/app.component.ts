import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { CalendarComponent } from 'ng-fullcalendar';
import { Options } from 'fullcalendar';
import { SchedulesClient, ScheduleDto, ShiftDto, ShiftType } from '../shared/service-proxy';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  calendarOptions: Options;
  title = 'Onyx Shift Scheduler';

  @ViewChild(CalendarComponent) ucCalendar: CalendarComponent;
  calendarEvents: Array<any>;
  currentSchedule: ScheduleDto;
  currentStats: string;

  constructor(
    injector: Injector,
    private schedulesClient: SchedulesClient
  ) {
    this.schedulesClient = injector.get(SchedulesClient);
  }

  getSchedule(): void {
    console.info('Fetching a new schedule.');
    this.schedulesClient.get().subscribe(result => {
      console.info('New schedule fetched.');

      this.currentSchedule = result;
      this.currentStats = result.statistics;

      console.info(result.statistics);

      this.calendarEvents = [];
      this.calendarEvents =
        result.shifts.filter(item => item.type !== ShiftType.Off).map((item: ShiftDto) => {
          return {
            title: item.employee,
            start: item.startDate,
            end: item.endDate,
            backgroundColor: (item.type === ShiftType.Day ? '#3c8dbc' : '#00a65a')
          };
        });

      console.info(this.calendarEvents);

      if (this.calendarEvents !== null)
        this.ucCalendar.fullCalendar('removeEvents');

      this.ucCalendar.fullCalendar('renderEvents', this.calendarEvents, true);
    });
  }

  ngOnInit() {
    this.calendarOptions = {
      editable: false,
      eventLimit: false,
      header: {
        left: 'prev,next today',
        center: 'title',
        right: 'month,agendaWeek,agendaDay,listMonth'
      }
    };
    this.getSchedule();
    this.calendarOptions.events = this.calendarEvents;
  }
}
