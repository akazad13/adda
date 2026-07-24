import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { firstValueFrom, Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../models/user';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { MemberMessagesComponent } from './member-messages/member-messages.component';
import { DatePipe } from '@angular/common';
import { DateAgoPipe } from '../../pipes/date-ago.pipe';

@Component({
    selector: 'app-member-detail',
    templateUrl: './member-detail.component.html',
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

    .profile-gallery-img {
      width: 100%;
      aspect-ratio: 1;
      object-fit: cover;
      border-radius: var(--adda-radius);
    }
  `,
    imports: [MemberMessagesComponent, TabsModule, DatePipe, DateAgoPipe]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs!: TabsetComponent;
  user!: User;
  // galleryOptions: NgxGalleryOptions[];
  // galleryImages: NgxGalleryImage[];
  routeSubscription!: Subscription;

  constructor(
    private readonly userService: UserService,
    private readonly authService: AuthService,
    private readonly notify: NotificationService,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.routeSubscription = this.route.data.subscribe((data) => {
      this.user = data['user'];
    });

    this.route.queryParams.subscribe((params) => {
      let selectedTab = parseInt(params['tab'], 10);
      if (isNaN(selectedTab) || selectedTab < 0 || selectedTab >= this.memberTabs.tabs.length) {
        selectedTab = 0;
      }
      this.memberTabs.tabs[selectedTab].active = true;
    });
  }

  ngOnDestroy() {
    this.routeSubscription?.unsubscribe();
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
  async bookmark(id: number): Promise<void> {
    try {
      await firstValueFrom(this.userService.bookmark(this.authService.decodedToken.nameid, id));
      this.notify.success('You have bookmarked: ' + this.user.knownAs);
    } catch (e: any) {
      this.notify.error(e.error);
    }
  }
}
