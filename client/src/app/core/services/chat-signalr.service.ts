import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { ChatMessageResponse } from '../models/community.model';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class ChatSignalRService {
  private readonly authService = inject(AuthService);
  private connection: signalR.HubConnection | null = null;
  private connectedToken: string | null = null;

  readonly messages = signal<ChatMessageResponse[]>([]);
  readonly connectionStatus = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');
  private currentHobbyId: number | null = null;

  async connect(): Promise<void> {
    const token = this.authService.getAccessToken();
    if (!token) return;

    // If connected with a different token (different user), disconnect first
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      if (this.connectedToken === token) return;
      await this.disconnect();
    }

    this.connectionStatus.set('connecting');

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.baseUrl}/hub/chat`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', (message: ChatMessageResponse) => {
      this.messages.update(msgs => [...msgs, message]);
    });

    this.connection.onreconnecting(() => this.connectionStatus.set('connecting'));
    this.connection.onreconnected(() => {
      this.connectionStatus.set('connected');
      if (this.currentHobbyId !== null) {
        this.connection?.invoke('JoinRoom', this.currentHobbyId);
      }
    });
    this.connection.onclose(() => this.connectionStatus.set('disconnected'));

    try {
      await this.connection.start();
      this.connectedToken = token;
      this.connectionStatus.set('connected');
    } catch (err) {
      console.error('SignalR connection failed:', err);
      this.connectionStatus.set('disconnected');
    }
  }

  async joinRoom(hobbyId: number): Promise<void> {
    if (this.currentHobbyId !== null && this.currentHobbyId !== hobbyId) {
      await this.leaveRoom(this.currentHobbyId);
    }
    this.currentHobbyId = hobbyId;
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('JoinRoom', hobbyId);
    }
  }

  async leaveRoom(hobbyId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('LeaveRoom', hobbyId);
    }
    if (this.currentHobbyId === hobbyId) {
      this.currentHobbyId = null;
    }
  }

  async sendMessage(hobbyId: number, content: string): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('SendMessage', hobbyId, content);
    }
  }

  setInitialMessages(messages: ChatMessageResponse[]): void {
    this.messages.set(messages);
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.connectedToken = null;
      this.connectionStatus.set('disconnected');
      this.currentHobbyId = null;
    }
  }
}
