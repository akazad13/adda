import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { Pagination, PaginatedResult } from '../models/pagination';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NotificationService } from '../services/notification.service';
import { DatePipe, LowerCasePipe, NgClass } from '@angular/common';
import { DateAgoPipe } from '../pipes/date-ago.pipe';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styles: ``,
    imports: [NgClass, DatePipe, DateAgoPipe, LowerCasePipe, FormsModule, PaginationModule, RouterLink]
})
export class MessagesComponent implements OnInit {
  messages: Message[] | null = null;
  pagination!: Pagination;
  messageContainer = 'unread';
  constructor(
    private readonly userService: UserService,
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute,
    private readonly notify: NotificationService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }

  async loadMessages(messageContainer?: string): Promise<void> {
    let msgContainer = this.messageContainer;
    if (messageContainer) {
      msgContainer = messageContainer;
    }

    try {
      const res: PaginatedResult<Message[]> = await firstValueFrom(
        this.userService.getMessages(
          this.authService.decodedToken.nameid,
          this.pagination.currentPage,
          this.pagination.itemsPerPage,
          msgContainer
        )
      );
      this.messages = res.result;
      this.pagination = res.pagination;
      if (messageContainer) {
        this.messageContainer = messageContainer;
      }
    } catch (e: any) {
      this.notify.error(e.statusText);
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  async deleteMessage(id: number): Promise<void> {
    this.notify.confirm('Are you sure you want to delete this message?', async () => {
      try {
        await firstValueFrom(this.userService.deleteMessage(id, this.authService.decodedToken.nameid));
        this.messages!.splice(
          this.messages!.findIndex((m) => m.id === id),
          1
        );
        this.notify.success('Message has been deleted');
      } catch (e: any) {
        this.notify.error('Failed to delete the messages');
      }
    });
  }
}
