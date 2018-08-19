import { NgModule } from '@angular/core';

import { environment } from '../environments/environment';
import * as ServiceProxy from './service-proxy';

export const apiBaseUrl = { provide: ServiceProxy.API_BASE_URL, useValue: environment.API_BASE_URL };

@NgModule({
  providers: [
    apiBaseUrl, ServiceProxy.SchedulesClient
  ]
})
export class ServiceProxyModule { }
