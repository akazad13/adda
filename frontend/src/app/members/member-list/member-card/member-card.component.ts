import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../../models/user';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { NotificationService } from '../../../services/notification.service';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styles: `
    .card-member {
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .card-img-wrapper {
      aspect-ratio: 1;
      overflow: hidden;
      border-radius: var(--adda-radius-lg) var(--adda-radius-lg) 0 0;
    }

    .member-photo {
      width: 100%;
      height: 100%;
      object-fit: cover;
      transition: transform 0.4s ease, filter 0.4s ease;
    }

    .card-member:hover .member-photo {
      transform: scale(1.08);
      filter: brightness(0.85);
    }

    .member-action-toolbar {
      opacity: 0;
      transform: translateY(12px);
      transition: all 0.3s ease;
      pointer-events: none;
    }

    .card-member:hover .member-action-toolbar {
      opacity: 1;
      transform: translateY(0);
      pointer-events: auto;
    }

    .fs-2xs { font-size: 0.7rem; }
    .fs-xs { font-size: 0.785rem; }
    .pointer-events-none { pointer-events: none; }
  `,
  imports: [RouterLink]
})
export class MemberCardComponent implements OnInit {
  @Input() user!: User;

  constructor(
    private readonly authService: AuthService,
    private readonly userService: UserService,
    private readonly notify: NotificationService
  ) {}

  ngOnInit() {}

  async bookmark(id: number | undefined): Promise<void> {
    if (id === undefined) return;

    try {
      await firstValueFrom(this.userService.bookmark(this.authService.decodedToken.nameid, id));
      this.notify.success('You have bookmarked: ' + this.user?.knownAs);
    } catch (e: any) {
      this.notify.error(e.error);
    }
  }
}
