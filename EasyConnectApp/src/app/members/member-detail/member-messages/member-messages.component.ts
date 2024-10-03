import { Component, OnInit, Input } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Message } from '../../../models/message';
import { AlertifyService } from '../../../services/alertify.service';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgFor, NgIf } from '@angular/common';
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
      height: 400px;
    }
  `,
  imports: [FormsModule, DatePipe, NgIf, NgFor],
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

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService
      .getMessageThread(currentUserId, this.recipientId)
      .pipe(
        tap((messages: Message[]) => {
          for (const message of messages) {
            if (message.isRead === false && message.recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, message.id); // to make all as read after view
            }
          }
        })
      )
      .subscribe(
        (messages) => {
          this.messages = messages;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe(
      (message: Message) => {
        this.messages.unshift(message);
        this.newMessage.content = '';
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
