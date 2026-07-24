import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styles: `
    .auth-page {
      min-height: calc(100vh - var(--adda-nav-height));
      display: flex;
      align-items: center;
      padding: 2rem 1rem 3rem;
    }
  `,
  imports: [FormsModule, RouterLink],
})
export class LoginComponent implements OnInit {
  model: { username?: string; password?: string } = {};

  constructor(
    private readonly authService: AuthService,
    private readonly notify: NotificationService,
    private readonly router: Router
  ) {}

  ngOnInit() {
    if (this.authService.loggedIn()) {
      this.router.navigate(['/members']);
    }
  }

  async login(): Promise<void> {
    try {
      await firstValueFrom(this.authService.login(this.model));
      this.notify.success('Logged in successfully!');
      this.router.navigate(['/members']);
    } catch (e: any) {
      this.notify.error(e.error?.title ?? 'Login failed');
    }
  }
}
