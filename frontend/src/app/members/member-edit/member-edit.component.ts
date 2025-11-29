import { Component, OnInit, ViewChild, HostListener, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { firstValueFrom, Subscription } from 'rxjs';
import { User } from '../../models/user';
import { AlertifyService } from '../../services/alertify.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { PhotoEditorComponent } from './photo-editor/photo-editor.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { DatePipe, NgIf } from '@angular/common';
import { DateAgoPipe } from '../../pipes/date-ago.pipe';

@Component({
    selector: 'app-member-edit',
    templateUrl: './member-edit.component.html',
    styles: `
   .img-thumbnail {
      margin: 25px;
      width: 85%;
      height: 85%;
    }

    .card-body {
      padding: 0 25px;
    }

    .card-footer {
      border-top: none;
    }

  `,
    imports: [PhotoEditorComponent, TabsModule, FormsModule, DatePipe, NgIf, DateAgoPipe]
})
export class MemberEditComponent implements OnInit, OnDestroy {
  constructor(
    private readonly route: ActivatedRoute,
    private readonly alertify: AlertifyService,
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
    this.routeSubscription.unsubscribe();
    this.currentPhotoUrlSubscription.unsubscribe();
  }

  async updateUser(): Promise<void> {
    try {
      await firstValueFrom(this.userService.updateUser(this.authService.decodedToken.nameid, this.user));
      this.alertify.success('Profile added successfully.');
      this.editFrom.reset(this.user);
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }
}
