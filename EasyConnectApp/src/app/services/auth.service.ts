import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Observable, BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../models/user';
import { environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = `${environment.apiUrl}/api/auth/`;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User | null = null;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private readonly http: HttpClient) {}

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  getCurrentUserId(): number {
    const decodedToken = this.jwtHelper.decodeToken(this.getToken());
    if (decodedToken == null) {
      return decodedToken;
    }
    return +decodedToken.nameid;
  }

  getToken(): string {
    const user = this.getStoredUser();
    if (user == null) {
      return user;
    }
    return user.token;
  }

  login(model: any): Observable<any> {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user;
          this.changeMemberPhoto(this.currentUser!.photoUrl);
        }
      })
    );
  }

  register(user: User): Observable<any> {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = this.getToken();
    return !this.jwtHelper.isTokenExpired(token);
  }

  roleMatch(allowedRoles: string[]): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;
    allowedRoles.forEach((element: string) => {
      if (userRoles.includes(element)) {
        isMatch = true;
      }
    });
    return isMatch;
  }

  private getStoredUser() {
    const storedUser = localStorage.getItem('user');
    return storedUser == null ? null : JSON.parse(storedUser);
  }
}
