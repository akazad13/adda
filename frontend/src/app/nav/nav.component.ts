import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
import { Subscription } from 'rxjs';
import { HasRoleDirective } from '../directives/hasRole.directive';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ChatService } from '../services/chat.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styles: `
    .dropdown-toggle,
    .dropdown-item {
      cursor: pointer;
    }

    .brand-icon {
      width: 34px;
      height: 34px;
      border-radius: 10px;
      background: rgba(255, 59, 92, 0.15);
      border: 1px solid rgba(255, 59, 92, 0.3);
      display: inline-flex;
      align-items: center;
      justify-content: center;
      font-size: 1.1rem;
    }

    .brand-text {
      font-family: var(--adda-font-display);
      font-size: 1.4rem;
      font-weight: 800;
      letter-spacing: -0.03em;
    }

    .nav-avatar {
      height: 32px;
      width: 32px;
      border-radius: 50%;
      object-fit: cover;
      border: 2px solid var(--adda-brand);
    }

    .max-w-120 {
      max-width: 120px;
      display: inline-block;
      vertical-align: middle;
    }

    .fs-xs {
      font-size: 0.75rem;
    }
  `,
  imports: [RouterLink, RouterLinkActive, HasRoleDirective, BsDropdownModule],
})
export class NavComponent implements OnInit, OnDestroy {
  photoUrl!: string;
  currentPhotoUrlSubscription!: Subscription;

  constructor(
    public authService: AuthService,
    private readonly notify: NotificationService,
    private readonly chatService: ChatService,
    private readonly router: Router
  ) {}

  ngOnInit() {
    this.currentPhotoUrlSubscription = this.authService.currentPhotoUrl.subscribe((photoUrl) => {
      this.photoUrl = photoUrl;
    });
  }

  ngOnDestroy() {
    this.currentPhotoUrlSubscription?.unsubscribe();
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    this.chatService.stopHubConnection();
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.notify.message('Logged Out!');
    this.router.navigate(['/']);
  }
}
