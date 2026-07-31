import { Component, OnInit, Input } from '@angular/core';
import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../environments/environment.prod';
import { Photo } from '../../../models/photo';
import { NotificationService } from '../../../services/notification.service';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { DecimalPipe, NgStyle } from '@angular/common';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-photo-editor',
    templateUrl: './photo-editor.component.html',
    styles: `
    .upload-zone-active {
      border-color: var(--adda-brand) !important;
      background: rgba(0,198,198,0.08) !important;
    }
    .not-approved {
      opacity: 0.35;
    }
  `,
    imports: [DecimalPipe, NgStyle]
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos!: Photo[];

  // Upload state
  selectedFiles: File[] = [];
  uploadProgress: number = 0;
  isUploading = false;
  isDragOver = false;

  baseUrl = `${environment.apiUrl}/api/`;

  constructor(
    private readonly authService: AuthService,
    private readonly userService: UserService,
    private readonly notify: NotificationService,
    private readonly http: HttpClient
  ) {}

  ngOnInit() {}

  // ─── Drag & Drop ───────────────────────────────────────────────────
  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
    const files = event.dataTransfer?.files;
    if (files) {
      this.addFiles(Array.from(files));
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.addFiles(Array.from(input.files));
      input.value = ''; // reset so same file can be re-selected
    }
  }

  private addFiles(files: File[]) {
    const allowed = files.filter(f => f.type.startsWith('image/'));
    const oversized = files.filter(f => f.size > 10 * 1024 * 1024);
    if (oversized.length) {
      this.notify.error(`${oversized.length} file(s) exceed the 10 MB limit and were skipped.`);
    }
    this.selectedFiles = [...this.selectedFiles, ...allowed.filter(f => f.size <= 10 * 1024 * 1024)];
  }

  removeFile(index: number) {
    this.selectedFiles.splice(index, 1);
    this.selectedFiles = [...this.selectedFiles];
  }

  clearQueue() {
    this.selectedFiles = [];
    this.uploadProgress = 0;
  }

  // ─── Upload ────────────────────────────────────────────────────────
  async uploadAll() {
    if (!this.selectedFiles.length || this.isUploading) return;

    const userId = this.authService.getCurrentUserId() || this.authService.decodedToken?.nameid;
    const token = this.authService.getToken();
    const url = `${this.baseUrl}users/${userId}/photos`;
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    this.isUploading = true;
    this.uploadProgress = 0;
    let uploaded = 0;
    const total = this.selectedFiles.length;

    for (const file of [...this.selectedFiles]) {
      const formData = new FormData();
      formData.append('file', file, file.name);

      try {
        const photo = await new Promise<Photo>((resolve, reject) => {
          this.http.post<Photo>(url, formData, {
            headers,
            reportProgress: true,
            observe: 'events',
          }).subscribe({
            next: (event) => {
              if (event.type === HttpEventType.UploadProgress && event.total) {
                const fileProgress = Math.round((event.loaded / event.total) * 100);
                this.uploadProgress = Math.round(((uploaded + fileProgress / 100) / total) * 100);
              } else if (event.type === HttpEventType.Response) {
                resolve(event.body as Photo);
              }
            },
            error: (err) => reject(err),
          });
        });

        this.photos.push(photo);
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          if (this.authService.currentUser) {
            this.authService.currentUser.photoUrl = photo.url;
            localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
          }
        }
        uploaded++;
        this.uploadProgress = Math.round((uploaded / total) * 100);
        this.notify.success(`"${file.name}" uploaded — pending review`);

      } catch (err: any) {
        const msg = err?.error || err?.message || 'Unknown error';
        this.notify.error(`Failed to upload "${file.name}": ${msg}`);
      }
    }

    this.isUploading = false;
    this.selectedFiles = [];
    setTimeout(() => (this.uploadProgress = 0), 2000);
  }

  // ─── Photo Actions ─────────────────────────────────────────────────
  async setMainPhoto(photo: Photo): Promise<void> {
    try {
      await firstValueFrom(this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id));
      this.photos.forEach(p => (p.isMain = false));
      photo.isMain = true;
      this.authService.changeMemberPhoto(photo.url);
      if (this.authService.currentUser) {
        this.authService.currentUser.photoUrl = photo.url;
        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      }
    } catch (e: any) {
      this.notify.error(e.statusText);
    }
  }

  async deletePhoto(id: number): Promise<void> {
    this.notify.confirm('Are you sure you want to delete this photo?', async () => {
      try {
        await firstValueFrom(this.userService.deletePhoto(this.authService.decodedToken.nameid, id));
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.notify.success('Photo has been deleted');
      } catch (e: any) {
        this.notify.error(e.statusText);
      }
    });
  }
}
