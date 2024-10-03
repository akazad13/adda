import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { firstValueFrom, Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../models/user';
import { AlertifyService } from '../../services/alertify.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { MemberMessagesComponent } from './member-messages/member-messages.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
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
      padding: 10px 15px;
      background-color: #fff;
      border-top: none;
    }
  `,
  imports: [MemberMessagesComponent, TabsModule, DatePipe],
  standalone: true,
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
    private readonly alertify: AlertifyService,
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
  async bookmark(id: number): Promise<void> {
    try {
      await firstValueFrom(this.userService.bookmark(this.authService.decodedToken.nameid, id));
      this.alertify.success('You have bookmarked: ' + this.user.knownAs);
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }
}
