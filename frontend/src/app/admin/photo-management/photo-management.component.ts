import { Component, OnInit } from '@angular/core';
import { Photo } from '../../models/photo';
import { AdminService } from '../../services/admin.service';
import { NotificationService } from '../../services/notification.service';

import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styles: ``,
  imports: []
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  constructor(private readonly adminService: AdminService, private readonly notify: NotificationService) { }

  ngOnInit() {
    this.getPhotosForModeration();
  }

  async getPhotosForModeration(): Promise<void> {
    try {
      const photos: Photo[] = await firstValueFrom(this.adminService.getPhotosForModeration());
      this.photos = photos;
    } catch (e: any) {
      this.notify.error(e.statusText);
    }
  }

  async approvePhoto(photoId: number): Promise<void> {
    try {
      await firstValueFrom(this.adminService.approvePhoto(photoId));
      this.photos.splice(
        this.photos.findIndex((p) => p.id === photoId),
        1
      );
      this.notify.success('Photo has been approved successfully');
    } catch (e: any) {
      this.notify.error(e.statusText);
    }
  }

  async rejectPhoto(photoId: number): Promise<void> {
    this.notify.confirm('Are you sure you want to delete this photo?', async () => {
      try {
        await firstValueFrom(this.adminService.rejectPhoto(photoId));
        this.photos.splice(
          this.photos.findIndex((p) => p.id === photoId),
          1
        );
        this.notify.success('Photo has been deleted successfully');
      } catch (e: any) {
        this.notify.error(e.statusText);
      }
    });
  }
}
