import { Component, OnInit, ViewChild, HostListener, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { firstValueFrom, Subscription } from 'rxjs';
import { User } from '../../models/user';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { PhotoEditorComponent } from './photo-editor/photo-editor.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { DatePipe } from '@angular/common';
import { DateAgoPipe } from '../../pipes/date-ago.pipe';

@Component({
    selector: 'app-member-edit',
    templateUrl: './member-edit.component.html',
    styles: `
    .profile-sidebar {
      overflow: hidden;
    }

    .profile-photo {
      width: 100%;
      aspect-ratio: 1;
      object-fit: cover;
    }

    .profile-meta div {
      margin-bottom: 0.85rem;
    }

    .profile-meta dt {
      font-size: 0.75rem;
      text-transform: uppercase;
      letter-spacing: 0.06em;
      color: var(--adda-text-muted);
      margin-bottom: 0.15rem;
    }

    .profile-meta dd {
      margin-bottom: 0;
    }
  `,
    imports: [PhotoEditorComponent, TabsModule, FormsModule, DatePipe, DateAgoPipe]
})
export class MemberEditComponent implements OnInit, OnDestroy {
  constructor(
    private readonly route: ActivatedRoute,
    private readonly notify: NotificationService,
    private readonly userService: UserService,
    private readonly authService: AuthService
  ) {}
  @ViewChild('editForm', { static: true }) editFrom!: NgForm;
  user!: User;
  photoUrl!: string;
  routeSubscription!: Subscription;
  currentPhotoUrlSubscription!: Subscription;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editFrom.dirty) {
      $event.returnValue = true;
    }
  }

  ngOnInit() {
    this.routeSubscription = this.route.data.subscribe((data) => {
      this.user = data['user'];
    });
    this.currentPhotoUrlSubscription = this.authService.currentPhotoUrl.subscribe((photoUrl) => (this.photoUrl = photoUrl));
  }

  ngOnDestroy() {
    this.routeSubscription?.unsubscribe();
    this.currentPhotoUrlSubscription?.unsubscribe();
  }

  async updateUser(): Promise<void> {
    try {
      await firstValueFrom(this.userService.updateUser(this.authService.decodedToken.nameid, this.user));
      this.notify.success('Profile added successfully.');
      this.editFrom.reset(this.user);
    } catch (e: any) {
      this.notify.error(e.statusText);
    }
  }
}
