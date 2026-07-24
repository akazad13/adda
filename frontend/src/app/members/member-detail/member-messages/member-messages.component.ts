import { Component, OnInit, Input, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Message } from '../../../models/message';
import { NotificationService } from '../../../services/notification.service';
import { AuthService } from '../../../services/auth.service';
import { UserService } from '../../../services/user.service';
import { FormsModule } from '@angular/forms';
import { DateAgoPipe } from '../../../pipes/date-ago.pipe';
import { firstValueFrom } from 'rxjs';
import { ChatService } from '../../../services/chat.service';
import { NgScrollbar } from 'ngx-scrollbar';
import { DatePipe, NgClass } from '@angular/common';
@Component({
    selector: 'app-member-messages',
    templateUrl: './member-messages.component.html',
    styles: `
    .chat-panel {
      border: 1px solid var(--adda-border);
    }

    .chat-body-wrap {
      min-height: clamp(280px, 50vh, 520px);
      max-height: clamp(320px, 55vh, 560px);
      padding: 0;
    }

    .chat {
      list-style: none;
      margin: 0;
      padding: 1rem 1rem 0.5rem;
    }

    .chat li {
      margin-bottom: 0.85rem;
      display: flex;
    }

    .chat li.chat-outgoing {
      justify-content: flex-end;
    }

    .chat-bubble {
      max-width: 85%;
      padding: 0.65rem 0.85rem;
      border-radius: var(--adda-radius);
      background: var(--adda-surface-raised);
      border: 1px solid var(--adda-border);
    }

    .chat-outgoing .chat-bubble {
      background: rgba(20, 154, 128, 0.15);
      border-color: rgba(20, 154, 128, 0.35);
    }

    .chat-bubble p {
      margin-bottom: 0.35rem;
    }

    .chat-scrollbar {
      --scrollbar-thumb-color: rgba(255, 255, 255, 0.25);
      height: 100%;
    }
  `,
    imports: [FormsModule, DateAgoPipe, NgScrollbar, NgClass]
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
    private readonly notify: NotificationService
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
      this.notify.error(e.statusText);
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
