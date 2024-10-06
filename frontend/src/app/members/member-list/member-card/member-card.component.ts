import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../../models/user';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { AlertifyService } from '../../../services/alertify.service';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styles: `
    .card:hover img {
    transform: scale(1.2, 1.2);
    transition-duration: 500ms;
    transition-timing-function: ease-out;
    opacity: 0.7;
  }

  .card img {
    transform: scale(1, 1);
    transition-duration: 500ms;
    transition-timing-function: ease-out;
  }

  .card-img-wrapper {
    overflow: hidden;
    position: relative;
  }

  .member-icons {
    position: absolute;
    bottom: -30%;
    left: 0;
    right: 0;
    margin-right: auto;
    margin-left: auto;
    opacity: 0;
  }

  .card-img-wrapper:hover .member-icons {
    bottom: 0;
    opacity: 1;
  }

  .animate {
    transition: all 0.3s ease-in-out;
  }
  `,
  imports: [RouterLink],
  standalone: true,
})
export class MemberCardComponent implements OnInit {
  @Input()
  user!: User;

  constructor(
    private readonly authService: AuthService,
    private readonly userService: UserService,
    private readonly alertify: AlertifyService
  ) {}

  ngOnInit() {}

  async bookmark(id: number | undefined): Promise<void> {
    if (id === undefined) return;

    try {
      await firstValueFrom(this.userService.bookmark(this.authService.decodedToken.nameid, id));
      this.alertify.success('You have bookmarked: ' + this.user?.knownAs);
    } catch (e: any) {
      this.alertify.error(e.error);
    }
  }
}
