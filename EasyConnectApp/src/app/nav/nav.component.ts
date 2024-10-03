import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { HasRoleDirective } from '../directives/hasRole.directive';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
  imports: [FormsModule, RouterLink, NgIf, HasRoleDirective],
  standalone: true,
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

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Logged in successfully!');
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('Logged Out!');
    this.router.navigate(['/home']);
  }
}
