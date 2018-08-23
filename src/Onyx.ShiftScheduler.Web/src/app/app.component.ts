import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { CalendarComponent } from 'ng-fullcalendar';
import { Options } from 'fullcalendar';
import { Moment } from 'moment';
import { SchedulesClient, ScheduleDto, ShiftDto, ShiftType, ScheduleRequestDto, TransitionSetsClient, TransitionSetDto } from '../shared/service-proxy';

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
  model: ScheduleRequestDto;
  transitionSets: TransitionSetDto[];
  currentSchedule: ScheduleDto;
  currentStats: string;

  constructor(
    injector: Injector,
    private schedulesClient: SchedulesClient,
    private transitionSetsClient: TransitionSetsClient,
  ) {
    this.schedulesClient = injector.get(SchedulesClient);
    this.transitionSetsClient = injector.get(TransitionSetsClient);

    // Defaults
    var today = new Date();
    today.setDate(today.getDate() + (1 + 7 - today.getDay()) % 7); // next monday
    this.model = new ScheduleRequestDto({
      transitionSetId: 1,
      startDate: today,
      days: 14,
      numberOfEmployees: 10,
      teamSize: 2,
      minShiftsPerCycle: 2,
      startHour: 7,
      shiftHours: 12
    });

    // Transition sets
    this.transitionSetsClient.getActiveTransitionSets().subscribe(result => {
      console.info('Transition sets fetched.');

      this.transitionSets = result;

      console.info(result);     
    });
  }

  getSchedule(): void {
    console.info('Fetching a new schedule.');
    this.schedulesClient.get(this.model).subscribe(result => {
      console.info('New schedule fetched.');

      this.currentSchedule = result;
      this.currentStats = result.statistics;

      console.info(result.statistics);

      if (result.error !== null) {
        this.ucCalendar.fullCalendar('removeEvents');
        return;
      }

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
