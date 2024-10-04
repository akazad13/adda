import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { environment } from '../environments/environment';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideScrollbarOptions } from 'ngx-scrollbar';

export function tokenGetter(): string | null {
  let userStr = localStorage.getItem('user');
  if (userStr != null) {
    const user = JSON.parse(userStr);
    if (user) return user.token;
    return null;
  }
  return null;
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
    provideScrollbarOptions({
      visibility: 'native',
      appearance: 'compact',
    }),
    importProvidersFrom([
      JwtModule.forRoot({
        config: {
          tokenGetter,
          allowedDomains: [environment.apiUrl.split('//')[1]], // needs to remove the https:// portion
          skipWhenExpired: true,
          disallowedRoutes: [`${environment.apiUrl.split('//')[1]}/api/auth`],
        },
      }),
    ]),
  ],
};
