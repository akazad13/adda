import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PaginatedResult } from '../models/pagination';

@Injectable()
export class ListsResolver  {
  pageNumber = 1;
  pageSize = 10;
  bookmarkParams = 'bookmarkeds';

  constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}
  resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<User[]> | null> {
    return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.bookmarkParams).pipe(
      catchError(error => {
        this.alertify.error('Problem retrieving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
