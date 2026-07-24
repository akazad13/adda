import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { NotificationService } from '../services/notification.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class MemberListResolver {
  pageNumber = 1;
  pageSize = 6;

  constructor(private readonly userService: UserService, private readonly router: Router, private readonly notify: NotificationService) {}
  resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<User[]> | null> {
    return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
      catchError((error) => {
        this.notify.error('Problem retrieving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
