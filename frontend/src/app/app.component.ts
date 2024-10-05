import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './services/auth.service';
import { User } from './models/user';
import { LoaderService } from './services/loader.service';
import { LoderComponent } from './shared/loader.component';
import { NavComponent } from './nav/nav.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [RouterOutlet, LoderComponent, NavComponent],
  standalone: true,
})
export class AppComponent {
  jwtHelper = new JwtHelperService();

  constructor(private readonly authService: AuthService, public loaderService: LoaderService) {}

  ngOnInit() {
    const userStr = localStorage.getItem('user');
    let user!: User;
    if (userStr != null) {
      user = JSON.parse(userStr);
    }
    if (user) {
      this.authService.currentUser = user;
      this.authService.changeMemberPhoto(user.photoUrl);
      if (user.token) {
        this.authService.decodedToken = this.jwtHelper.decodeToken(user.token);
      }
    }
  }
}
