import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: `
    .hero-section {
      min-height: calc(100vh - var(--adda-nav-height));
      display: flex;
      align-items: center;
    }

    .hero-glow-orb {
      width: 280px;
      height: 280px;
      background: radial-gradient(circle, rgba(255, 59, 92, 0.25) 0%, rgba(0, 242, 254, 0.15) 50%, transparent 70%);
      filter: blur(40px);
      border-radius: 50%;
      pointer-events: none;
    }

    .width-48 { width: 48px; }
    .height-48 { height: 48px; }
    .object-cover { object-fit: cover; }
    .max-w-600 { max-width: 600px; }
    .fs-xs { font-size: 0.75rem; }
    .fs-sm { font-size: 0.875rem; }
  `,
  imports: [RouterLink],
})
export class HomeComponent implements OnInit {
  constructor(public authService: AuthService) {}

  ngOnInit() {}
}
