import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { firstValueFrom, Subscription } from 'rxjs';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { AlertifyService } from '../../services/alertify.service';
import { PaginatedResult, Pagination } from '../../models/pagination';
import { NgClass } from '@angular/common';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from './member-card/member-card.component';

@Component({
    selector: 'app-member-list',
    templateUrl: './member-list.component.html',
    styles: ``,
    imports: [PaginationModule, FormsModule, NgClass, MemberCardComponent]
})
export class MemberListComponent implements OnInit, OnDestroy {
  users: User[] = [];
  userParams: any = {
    minAge: 18,
    maxAge: 99,
    gender: 'male',
    orderBy: 'lastActive',
    pageNumber: 1,
    pageSize: 10,
  };
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];
  pagination!: Pagination;
  routeSubscription!: Subscription;
  constructor(
    private readonly userService: UserService,
    private readonly alertify: AlertifyService,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.routeSubscription = this.route.data.subscribe((data) => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
  }

  ngOnDestroy() {
    this.routeSubscription.unsubscribe();
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers(this.userParams.orderBy);
  }

  resetFilters() {
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers(this.userParams.orderBy);
  }

  async loadUsers(orderBy: string | null): Promise<void> {
    this.userParams.orderBy = orderBy ?? this.userParams.orderBy;

    try {
      const res: PaginatedResult<User[]> = await firstValueFrom(
        this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      );
      this.users = res.result!;
      this.pagination = res.pagination;
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }
}
