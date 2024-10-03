import { Component } from '@angular/core';

@Component({
  selector: 'app-loader',
  template: '<div class="overlay"><div class="loader">Loading...</div></div>',
  styleUrls: ['./loader.component.css'],
  standalone: true,
})
export class LoderComponent {}
