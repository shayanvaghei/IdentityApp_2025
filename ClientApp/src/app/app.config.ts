import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideToastr } from 'ngx-toastr';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { CoreService } from './core/core.service';
import { lastValueFrom } from 'rxjs';
import { credentialInterceptor } from './core/interceptors/credential-interceptor';
import { errorInterceptor } from './core/interceptors/error-interceptor';
import { TimeagoModule } from 'ngx-timeago';
import { importProvidersFrom } from '@angular/core';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    importProvidersFrom(TimeagoModule.forRoot()),
    provideToastr({
      timeOut: 3000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true
    }),
    provideHttpClient(withInterceptors([credentialInterceptor, errorInterceptor])),
    provideAppInitializer(async () => {
      // for the start of the application we call the following method from coreService
      const coreService = inject(CoreService);
      return lastValueFrom(coreService.initializeApp()).finally(() => {

      })
    })
  ]
};
