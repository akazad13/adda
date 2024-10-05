import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}
  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild!.data['roles'] as Array<string>;

    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.alertify.error('You are not authorized to access this data');
      }
    }

    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertify.error('You are not authorized!!');
    this.router.navigate(['home']);
    return false;
  }
}
