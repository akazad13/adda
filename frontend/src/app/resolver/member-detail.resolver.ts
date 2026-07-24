import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { NotificationService } from '../services/notification.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MemberDetailResolver {
  constructor(private readonly userService: UserService, private readonly router: Router, private readonly notify: NotificationService) {}
  resolve(route: ActivatedRouteSnapshot): Observable<User | null> {
    return this.userService.getUser(route.params['id']).pipe(
      catchError((error) => {
        this.notify.error('Problem retrieving data');
        this.router.navigate(['/members']);
        return of(null);
      })
    );
  }
}
