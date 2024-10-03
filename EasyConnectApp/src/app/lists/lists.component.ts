import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from '../models/pagination';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { MemberCardComponent } from '../members/member-list/member-card/member-card.component';
import { NgClass, NgFor } from '@angular/common';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styles: ``,
  imports: [FormsModule, PaginationModule, MemberCardComponent, NgFor, NgClass],
  standalone: true,
})
export class ListsComponent implements OnInit {
  users: User[] | null = null;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };
  bookmarkParam: string = 'bookmarkeds';
  constructor(
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
    this.loadUsers(null);
  }

  async loadUsers(bookmarkOption: string | null): Promise<void> {
    if (bookmarkOption != null) {
      this.bookmarkParam = bookmarkOption;
    }
    try {
      const res: PaginatedResult<User[]> = await firstValueFrom(
        this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.bookmarkParam)
      );
      if (res.result != null) {
        this.users = res.result;
      }
      this.pagination = res.pagination;
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }
}
