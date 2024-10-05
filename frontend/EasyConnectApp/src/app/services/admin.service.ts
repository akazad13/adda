import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user';
import { Photo } from '../models/photo';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = `${environment.apiUrl}/api/`;
  constructor(private readonly http: HttpClient) {}

  getUsersWithRoles(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'admin/usersWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
  }

  getPhotosForModeration() {
    return this.http.get<Photo[]>(this.baseUrl + 'admin/photosForModeration');
  }

  approvePhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'admin/photo/' + photoId, {});
  }

  rejectPhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'admin/photo/' + photoId);
  }
}
