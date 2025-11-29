import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { Pagination, PaginatedResult } from '../models/pagination';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styles: `
    table {
      margin-top: 15px;
    }
    .img-circle {
      max-height: 50px;
    }
  `,
    imports: [NgIf, NgFor, NgClass, DatePipe, FormsModule, PaginationModule, RouterLink]
})
export class MessagesComponent implements OnInit {
  messages: Message[] | null = null;
  pagination!: Pagination;
  messageContainer = 'unread';
  constructor(
    private readonly userService: UserService,
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute,
    private readonly alertify: AlertifyService
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
      this.alertify.error(e.statusText);
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  async deleteMessage(id: number): Promise<void> {
    this.alertify.confirm('Are you sure you want to delete this message?', async () => {
      try {
        await firstValueFrom(this.userService.deleteMessage(id, this.authService.decodedToken.nameid));
        this.messages!.splice(
          this.messages!.findIndex((m) => m.id === id),
          1
        );
        this.alertify.success('Message has been deleted');
      } catch (e: any) {
        this.alertify.error('Failed to delete the messages');
      }
    });
  }
}
