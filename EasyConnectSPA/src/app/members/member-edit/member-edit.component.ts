import { Component, OnInit, ViewChild, HostListener, OnDestroy } from '@angular/core';
import { User } from 'src/app/models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit, OnDestroy {
  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService
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

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(
      (next) => {
        this.alertify.success('Profile added successfully.');
        this.editFrom.reset(this.user);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
