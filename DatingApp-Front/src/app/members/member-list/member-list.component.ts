import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { AlertifyService } from '../../services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit, OnDestroy {
  users: User[];
  routeSubscription: Subscription;
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.routeSubscription = this.route.data.subscribe(data => {
      this.users = data.users;
    });
  }
  ngOnDestroy() {
    this.routeSubscription.unsubscribe();
  }
}
