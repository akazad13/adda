import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';
import { AlertifyService } from 'src/app/services/alertify.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs!: TabsetComponent;
  user!: User;
  // galleryOptions: NgxGalleryOptions[];
  // galleryImages: NgxGalleryImage[];
  routeSubscription!: Subscription;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.routeSubscription = this.route.data.subscribe((data) => {
      this.user = data['user'];
    });

    this.route.queryParams.subscribe((params) => {
      const selectedTab = params['tab'];
      this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
    });
  }

  ngOnDestroy() {
    this.routeSubscription.unsubscribe();
  }

  getImages() {
    const imageUrl = [];
    for (const photo of this.user.photos!) {
      imageUrl.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description,
      });
    }
    return imageUrl;
  }

  selecTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }
  bookmark(id: number) {
    this.userService.bookmark(this.authService.decodedToken.nameid, id).subscribe(
      (data) => {
        this.alertify.success('You have bookmarked: ' + this.user.knownAs);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
