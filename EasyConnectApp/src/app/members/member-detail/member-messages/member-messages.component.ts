import { Component, OnInit, Input } from '@angular/core';
import { Message } from '../../../models/message';
import { AlertifyService } from '../../../services/alertify.service';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { DateAgoPipe } from '../../../pipes/date-ago.pipe';
import { firstValueFrom } from 'rxjs';
@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styles: `
    .card {
      border: none;
    }

    .chat {
      list-style: none;
      margin: 0;
      padding: 0;
    }

    .chat li {
      margin-bottom: 10px;
      padding-bottom: 10px;
      border-bottom: 1px dotted #b3a9a9;
    }

    .rounded-circle {
      height: 50px;
      width: 50px;
    }

    .card-body {
      overflow-y: scroll;
      height: 460px;
    }
  `,
  imports: [FormsModule, DatePipe, NgIf, NgFor, DateAgoPipe],
  standalone: true,
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId!: number;
  messages!: Message[];
  newMessage: any = {};

  constructor(
    private readonly userService: UserService,
    private readonly authService: AuthService,
    private readonly alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.loadMessages();
  }

  async loadMessages(): Promise<void> {
    const currentUserId = +this.authService.decodedToken.nameid;
    try {
      const messages: Message[] = await firstValueFrom(this.userService.getMessageThread(currentUserId, this.recipientId));
      for (const message of messages) {
        if (message.isRead === false && message.recipientId === currentUserId) {
          this.userService.markAsRead(currentUserId, message.id); // to make all as read after view
        }
      }
      this.messages = messages;
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }

  async sendMessage(): Promise<void> {
    this.newMessage.recipientId = this.recipientId;
    try {
      const message: Message = await firstValueFrom(this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage));
      this.messages.push(message);
      this.newMessage.content = '';
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }
}
