import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { firstValueFrom, Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { HasRoleDirective } from '../directives/hasRole.directive';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styles: `
    .dropdown-toggle,
    .dropdown-item {
      cursor: pointer;
    }

    img {
      max-height: 50px;
      border: 2px solid #fff;
      display: inline;
    }
  `,
    imports: [FormsModule, RouterLink, NgIf, HasRoleDirective, BsDropdownModule]
})
export class NavComponent implements OnInit, OnDestroy {
  model: any = {};
  photoUrl!: string;
  currentPhotoUrlSubscription!: Subscription;

  constructor(public authService: AuthService, private readonly alertify: AlertifyService, private readonly router: Router) {}

  ngOnInit() {
    this.currentPhotoUrlSubscription = this.authService.currentPhotoUrl.subscribe((photoUrl) => {
      this.photoUrl = photoUrl;
    });
  }

  ngOnDestroy() {
    this.currentPhotoUrlSubscription.unsubscribe();
  }

  async login(): Promise<void> {
    try {
      await firstValueFrom(this.authService.login(this.model));
      this.alertify.success('Logged in successfully!');
    } catch (e: any) {
      this.alertify.error(e.error.title);
    } finally {
      this.router.navigate(['/members']);
    }
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('Logged Out!');
    this.router.navigate(['/home']);
  }
}
