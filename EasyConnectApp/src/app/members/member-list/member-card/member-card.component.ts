import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../../models/user';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { AlertifyService } from '../../../services/alertify.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  imports: [RouterLink],
  standalone: true,
})
export class MemberCardComponent implements OnInit {
  @Input() user: User | null = null;

  constructor(
    private readonly authService: AuthService,
    private readonly userService: UserService,
    private readonly alertify: AlertifyService
  ) {}

  ngOnInit() {}

  bookmark(id: number | undefined) {
    if (id === undefined) return;
    this.userService.bookmark(this.authService.decodedToken.nameid, id).subscribe(
      (data) => {
        this.alertify.success('You have bookmarked: ' + this.user?.knownAs);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
