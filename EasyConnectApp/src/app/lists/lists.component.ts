import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from '../models/pagination';
import { User } from '../models/user';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { MemberCardComponent } from '../members/member-list/member-card/member-card.component';
import { NgFor } from '@angular/common';
import { ListsResolver } from '../resolver/lists.resolver';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
  imports: [FormsModule, PaginationModule, MemberCardComponent, NgFor],
  standalone: true,
})
export class ListsComponent implements OnInit {
  users: User[] | null = null;
  pagination!: Pagination;
  bookmarkParam!: string;
  constructor(
    private readonly authService: AuthService,
    private readonly userService: UserService,
    private readonly route: ActivatedRoute,
    private readonly alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.bookmarkParam = 'bookmarkeds';
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.bookmarkParam).subscribe(
      (res: PaginatedResult<User[]>) => {
        if (res.result != null) {
          this.users = res.result;
        }
        this.pagination = res.pagination;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
