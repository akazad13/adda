import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/models/photo';
import { AdminService } from 'src/app/services/admin.service';
import { AlertifyService } from 'src/app/services/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  constructor(private adminService: AdminService, private alertify: AlertifyService) {}

  ngOnInit() {
    this.getPhotosForModeration();
  }

  getPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe(
      (photos: Photo[]) => {
        this.photos = photos;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(
      () => {
        this.photos.splice(
          this.photos.findIndex(p => p.id === photoId),
          1
        );
        this.alertify.success('Photo has been approved successfully');
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  rejectPhoto(photoId: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.adminService.rejectPhoto(photoId).subscribe(
        () => {
          this.photos.splice(
            this.photos.findIndex(p => p.id === photoId),
            1
          );
          this.alertify.success('Photo has been deleted successfully');
        },
        error => {
          this.alertify.error(error);
        }
      );
    });
  }
}
