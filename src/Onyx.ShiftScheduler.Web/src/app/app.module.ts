import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { environment } from '../environments/environment';

import { FullCalendarModule } from 'ng-fullcalendar';

import { ServiceProxyModule } from '../shared/service-proxy.module';
import * as ServiceProxy from '../shared/service-proxy';

import { AppComponent } from './app.component';

export const apiBaseUrl = { provide: ServiceProxy.API_BASE_URL, useValue: environment.API_BASE_URL };

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FullCalendarModule,
    ServiceProxyModule
  ],
  providers: [apiBaseUrl, ServiceProxy.SchedulesClient],
  bootstrap: [AppComponent]
})
export class AppModule { }
