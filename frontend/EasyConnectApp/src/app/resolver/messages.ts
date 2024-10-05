import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../models/message';
import { AuthService } from '../services/auth.service';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class MessagesResolver {
  pageNumber = 1;
  pageSize = 10;
  messageContainer = 'unread';

  constructor(
    private readonly userService: UserService,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly alertify: AlertifyService
  ) {}
  resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<Message[]> | null> {
    return this.userService.getMessages(this.authService.decodedToken.nameid, this.pageNumber, this.pageSize, this.messageContainer).pipe(
      catchError((error) => {
        this.alertify.error('Problem retrieving messages');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
