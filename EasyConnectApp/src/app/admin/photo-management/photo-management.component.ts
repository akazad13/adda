import { Component, OnInit } from '@angular/core';
import { Photo } from '../../models/photo';
import { AdminService } from '../../services/admin.service';
import { AlertifyService } from '../../services/alertify.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styles: ``,
  imports: [NgFor],
  standalone: true,
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  constructor(private readonly adminService: AdminService, private readonly alertify: AlertifyService) {}

  ngOnInit() {
    this.getPhotosForModeration();
  }

  getPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe(
      (photos: Photo[]) => {
        this.photos = photos;
      },
      (error: any) => {
        this.alertify.error(error);
      }
    );
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(
      () => {
        this.photos.splice(
          this.photos.findIndex((p) => p.id === photoId),
          1
        );
        this.alertify.success('Photo has been approved successfully');
      },
      (error: any) => {
        this.alertify.error(error);
      }
    );
  }

  rejectPhoto(photoId: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.adminService.rejectPhoto(photoId).subscribe(
        () => {
          this.photos.splice(
            this.photos.findIndex((p) => p.id === photoId),
            1
          );
          this.alertify.success('Photo has been deleted successfully');
        },
        (error: any) => {
          this.alertify.error(error);
        }
      );
    });
  }
}
