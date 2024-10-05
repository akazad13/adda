import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { LoaderService } from './loader.service';
import { Message } from '../models/message';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  hubUrl = environment.apiUrl + '/hubs/';
  private hubConnection!: HubConnection;
  message$ = new Subject<Message>();

  constructor(private readonly loaderService: LoaderService, private readonly authService: AuthService) {}

  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'chat?', {
        accessTokenFactory: () => this.authService.getToken(),
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((error) => console.log(error))
      .finally(() => {});

    this.hubConnection.on('NewMessage', (message) => {
      this.message$.next(message);
    });
  }
  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((error) => console.log(error));
    }
  }

  async sendMessage(serderId: number, recipientId: number, content: string) {
    return this.hubConnection
      .invoke('SendMessageAsync', {
        SenderId: serderId,
        RecipientId: recipientId,
        Content: content,
      })
      .catch((error) => console.log(error));
  }
  async readThreadMessage(recipientId: number) {
    return this.hubConnection.invoke('ReadThreadMessageAsync', { RecipientId: +recipientId }).catch((error) => console.log(error));
  }
}
