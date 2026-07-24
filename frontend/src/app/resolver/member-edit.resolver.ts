import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { NotificationService } from '../services/notification.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class MemberEditResolver {
  constructor(
    private readonly userService: UserService,
    private readonly router: Router,
    private readonly notify: NotificationService,
    private readonly authService: AuthService
  ) {}
  resolve(route: ActivatedRouteSnapshot): Observable<User | null> {
    return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
      catchError((error) => {
        this.notify.error('Problem retrieving your data');
        this.router.navigate(['/members']);
        return of(null);
      })
    );
  }
}
