import { Component, OnInit, Input } from '@angular/core';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { environment } from '../../../../environments/environment.prod';
import { Photo } from '../../../models/photo';
import { AlertifyService } from '../../../services/alertify.service';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle, SlicePipe } from '@angular/common';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styles: `
    img.img-thumbnail {
      height: 100px;
      min-width: 100px !important;
      margin-bottom: 2px;
    }

    .nv-file-over {
      border: dotted 3px red;
    }

    input[type='file'] {
      color: transparent;
    }

    .pending-image-text {
      color: red;
      position: absolute;
      top: 35px;
      left: 7px;
      text-align: center;
    }

    .not-approved {
      opacity: 0.2;
    }
  `,
  imports: [NgIf, NgStyle, FileUploadModule, DecimalPipe, SlicePipe, NgClass, NgFor],
  standalone: true,
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos!: Photo[];
  uploader!: FileUploader;
  hasBaseDropZoneOver: boolean;
  baseUrl = `${environment.apiUrl}/api/`;
  currentMain!: Photo;

  constructor(
    private readonly authService: AuthService,
    private readonly userService: UserService,
    private readonly alertify: AlertifyService
  ) {
    this.hasBaseDropZoneOver = false;
  }

  ngOnInit() {
    this.initializeUploder();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploder() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    // To pass cors check
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain,
          isApproved: res.isApproved,
        };
        this.photos.push(photo);
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser!.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(
      () => {
        this.currentMain = this.photos.filter((p) => p.isMain === true)[0];
        this.currentMain.isMain = false;
        photo.isMain = true;
        this.authService.changeMemberPhoto(photo.url);
        this.authService.currentUser!.photoUrl = photo.url;
        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  deletePhoto(id: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(
        () => {
          this.photos.splice(
            this.photos.findIndex((p) => p.id === id),
            1
          );
          this.alertify.success('Photo has been deleted');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
    });
  }
}
