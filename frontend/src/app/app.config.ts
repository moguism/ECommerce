import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { provideNgxStripe } from 'ngx-stripe';


export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes), provideHttpClient(), provideNgxStripe('pk_test_51QJzjI2MpRBL4z2CrbGuFG2kZiSsvm4m9YeFlTP1gX5wQUQiwnGY1N95kQTaqb3xvsvUraEqzhXINwXsxBAKkaGZ00JRwU6xVf')]
};
