import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../models/user';
import { PaginatedResult } from '../models/pagination';
import { Message } from '../models/message';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = `${environment.apiUrl}/api/`;

  constructor(private http: HttpClient) {}

  getUsers(page?: any, itemsPerPage?: any, userParams?: any, bookmarkParams?: string): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (bookmarkParams === 'bookmarkers') {
      params = params.append('bookmarkers', 'true');
    } else if (bookmarkParams === 'bookmarkeds') {
      params = params.append('bookmarkeds', 'true');
    }

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        const pagination = response.headers.get('Pagination');
        if (pagination != null) {
          paginatedResult.pagination = JSON.parse(pagination);
        }

        return paginatedResult;
      })
    );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User): Observable<any> {
    return this.http.put<User>(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  bookmark(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/bookmark/' + recipientId, {});
  }

  getMessages(id: number, page?: number, itemsPerPage?: number, messageContainer?: string) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    if (messageContainer != null) {
      params = params.append('MessageContainer', messageContainer);
    }

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        const pagination = response.headers.get('Pagination');
        if (pagination != null) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
  }

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message): Observable<Message> {
    return this.http.post<Message>(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
  }

  markAsRead(userId: number, messageId: number) {
    this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {}).subscribe();
  }
}
