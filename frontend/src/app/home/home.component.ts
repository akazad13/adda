import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ``,
  imports: [NgIf],
  standalone: true,
})
export class HomeComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
