import { Component, OnInit, Input, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Message } from '../../../models/message';
import { AlertifyService } from '../../../services/alertify.service';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { DateAgoPipe } from '../../../pipes/date-ago.pipe';
import { firstValueFrom } from 'rxjs';
import { ChatService } from '../../../services/chat.service';
import { NgScrollbar } from 'ngx-scrollbar';
@Component({
    selector: 'app-member-messages',
    templateUrl: './member-messages.component.html',
    styles: `
    .card {
      border: none;
    }

    .card-body {
      height: 644px;
    }

    .chat {
      list-style: none;
      margin: 0;
      padding: 0 10px 0 0;
    }

    .chat li {
      margin-bottom: 10px;
      padding-bottom: 10px;
      border-bottom: 1px dotted #b3a9a9;
    }

    .chat-body p {
      margin-bottom: 2px;
    }

    .rounded-circle {
      height: 50px;
      width: 50px;
    }

    .chat-scrollbar {
      --scrollbar-thumb-color: rgba(0, 0, 0, 0.4)
    }
  `,
    imports: [FormsModule, DatePipe, DateAgoPipe, NgScrollbar]
})
export class MemberMessagesComponent implements OnInit, OnDestroy {
  @ViewChild(NgScrollbar) scrollable!: NgScrollbar;

  @Input() recipientId!: number;
  messages!: Message[];
  newMessage: string = '';

  constructor(
    private readonly userService: UserService,
    private readonly authService: AuthService,
    private readonly chatService: ChatService,
    private readonly alertify: AlertifyService
  ) {
    this.chatService.createHubConnection();
  }

  ngOnInit() {
    this.loadMessages();

    this.chatService.message$.subscribe((message: Message) => {
      this.messages = [...this.messages, message];
    });
  }

  async loadMessages(): Promise<void> {
    const currentUserId = +this.authService.decodedToken.nameid;
    try {
      const messages: Message[] = await firstValueFrom(this.userService.getMessageThread(currentUserId, this.recipientId));
      this.messages = messages;
      this.scrollToLatestMessage();
      setTimeout(async () => {
        await this.chatService.readThreadMessage(this.recipientId);
      }, 100);
    } catch (e: any) {
      this.alertify.error(e.statusText);
    }
  }

  async sendMessage(): Promise<void> {
    await this.chatService.sendMessage(this.authService.getCurrentUserId(), this.recipientId, this.newMessage);
    this.newMessage = '';
    this.scrollToLatestMessage();
  }

  ngOnDestroy(): void {
    this.chatService.stopHubConnection();
  }

  private scrollToLatestMessage(): void {
    setTimeout(async () => {
      this.scrollable.scrollTo({ top: this.scrollable.nativeElement.scrollHeight });
    }, 0);
  }
}
