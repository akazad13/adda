import { Component, OnInit } from '@angular/core';
import { PhotoManagementComponent } from '../photo-management/photo-management.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagementComponent } from '../user-management/user-management.component';
import { HasRoleDirective } from '../../directives/hasRole.directive';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css'],
  imports: [PhotoManagementComponent, TabsModule, UserManagementComponent, HasRoleDirective],
  standalone: true,
})
export class AdminPanelComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
