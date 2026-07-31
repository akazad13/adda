import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { LoaderService } from './loader.service';
import { Message } from '../models/message';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  hubUrl = environment.apiUrl + '/hubs/';
  private hubConnection!: HubConnection;
  message$ = new Subject<Message>();

  constructor(
    private readonly loaderService: LoaderService,
    private readonly authService: AuthService,
    private readonly notify: NotificationService
  ) {}

  createHubConnection() {
    if (this.hubConnection && (this.hubConnection.state === HubConnectionState.Connected || this.hubConnection.state === HubConnectionState.Connecting)) {
      return;
    }

    const token = this.authService.getToken();
    if (!token) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'chat', {
        accessTokenFactory: () => this.authService.getToken(),
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((error) => console.log(error));

    this.hubConnection.on('NewMessage', (message: Message) => {
      const currentUserId = this.authService.getCurrentUserId();
      if (currentUserId && message.senderId !== currentUserId) {
        const sender = message.senderKnownAs || 'Someone';
        this.notify.message(`New message from ${sender}: ${message.content}`);
      }
      this.message$.next(message);
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((error) => console.log(error));
    }
  }

  async sendMessage(senderId: number, recipientId: number, content: string) {
    return this.hubConnection
      .invoke('SendMessageAsync', {
        SenderId: senderId,
        RecipientId: recipientId,
        Content: content,
      })
      .catch((error) => console.log(error));
  }

  async readThreadMessage(recipientId: number) {
    return this.hubConnection.invoke('ReadThreadMessageAsync', { RecipientId: +recipientId }).catch((error) => console.log(error));
  }
}
