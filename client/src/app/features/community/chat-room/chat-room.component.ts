import {
  Component,
  OnInit,
  OnDestroy,
  AfterViewChecked,
  ViewChild,
  ElementRef,
  signal,
  inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommunityService } from '../../../core/services/community.service';
import { ChatSignalRService } from '../../../core/services/chat-signalr.service';
import { HobbyService } from '../../../core/services/hobby.service';
import { AuthService } from '../../../core/services/auth.service';
import { HobbyResponse } from '../../../core/models/hobby.model';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../../shared/components/ember-background/ember-background.component';
import { getImageUrl } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-chat-room',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './chat-room.component.html',
  styleUrl: './chat-room.component.scss'
})
export class ChatRoomComponent implements OnInit, OnDestroy, AfterViewChecked {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly communityService = inject(CommunityService);
  protected readonly chatService = inject(ChatSignalRService);
  private readonly hobbyService = inject(HobbyService);
  private readonly authService = inject(AuthService);
  private readonly destroy$ = new Subject<void>();

  @ViewChild('messageContainer') messageContainer!: ElementRef<HTMLDivElement>;

  protected readonly hobby = signal<HobbyResponse | null>(null);
  protected readonly isLoading = signal(true);
  protected messageText = '';

  private hobbyId!: number;
  private shouldScroll = false;

  protected get currentUserId(): number | undefined {
    return this.authService.currentUser()?.id;
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('hobbyId');
    if (!idParam) {
      this.router.navigate(['/community']);
      return;
    }
    this.hobbyId = +idParam;

    // Load hobby info
    this.hobbyService.getHobbyById(this.hobbyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (hobby) => this.hobby.set(hobby),
        error: () => this.router.navigate(['/community'])
      });

    // Load chat history then connect SignalR
    this.communityService.getChatMessages(this.hobbyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: async (result) => {
          this.chatService.setInitialMessages(result.items);
          this.isLoading.set(false);
          this.shouldScroll = true;

          await this.chatService.connect();
          await this.chatService.joinRoom(this.hobbyId);
        },
        error: () => this.isLoading.set(false)
      });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.scrollToBottom();
      this.shouldScroll = false;
    }
  }

  ngOnDestroy(): void {
    if (this.hobbyId) {
      this.chatService.leaveRoom(this.hobbyId);
    }
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected async sendMessage(): Promise<void> {
    const content = this.messageText.trim();
    if (!content || !this.hobbyId) return;

    await this.chatService.sendMessage(this.hobbyId, content);
    this.messageText = '';
    this.shouldScroll = true;
  }

  protected getImageUrl = getImageUrl;

  protected getInitialsFromName(fullName: string): string {
    const parts = fullName.split(' ');
    const first = parts[0]?.charAt(0) || '';
    const last = parts[1]?.charAt(0) || '';
    return `${first}${last}`.toUpperCase();
  }

  protected formatTime(dateStr: string): string {
    const date = new Date(dateStr);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  protected formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  }

  private scrollToBottom(): void {
    if (this.messageContainer) {
      const el = this.messageContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    }
  }
}
