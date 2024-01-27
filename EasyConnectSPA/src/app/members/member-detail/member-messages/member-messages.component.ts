import { Component, OnInit, Input } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Message } from 'src/app/models/message';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { AlertifyService } from 'src/app/services/alertify.service';
@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId!: number;
  messages!: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) {}

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
