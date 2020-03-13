import { Injectable } from '@angular/core';

@Injectable()
export class LoaderService {
  viewLoader = false;
  showLoader() {
    this.viewLoader = true;
  }
  hideLoader() {
    this.viewLoader = false;
  }
}
