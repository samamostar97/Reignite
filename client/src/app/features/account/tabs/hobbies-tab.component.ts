import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService } from '../../../core/services/profile.service';
import { HobbyService } from '../../../core/services/hobby.service';
import { NotificationService } from '../../../core/services/notification.service';
import { UserHobbyResponse, SkillLevel } from '../../../core/models/user.model';
import { HobbyResponse } from '../../../core/models/hobby.model';

@Component({
  selector: 'app-hobbies-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './hobbies-tab.component.html',
  styleUrl: './hobbies-tab.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HobbiesTabComponent implements OnInit {
  private readonly profileService = inject(ProfileService);
  private readonly hobbyService = inject(HobbyService);
  private readonly notificationService = inject(NotificationService);

  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly userHobbies = signal<UserHobbyResponse[]>([]);
  protected readonly allHobbies = signal<HobbyResponse[]>([]);
  protected readonly showAddForm = signal(false);
  protected readonly selectedHobbyId = signal<number | null>(null);
  protected readonly selectedSkillLevel = signal<SkillLevel>(SkillLevel.Beginner);

  protected readonly SkillLevel = SkillLevel;

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    this.isLoading.set(true);
    this.profileService.getHobbies().subscribe({
      next: (hobbies) => {
        this.userHobbies.set(hobbies);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });

    this.hobbyService.getHobbies().subscribe({
      next: (hobbies) => this.allHobbies.set(hobbies)
    });
  }

  protected get availableHobbies(): HobbyResponse[] {
    const userHobbyIds = this.userHobbies().map(h => h.hobbyId);
    return this.allHobbies().filter(h => !userHobbyIds.includes(h.id));
  }

  protected getSkillLabel(level: SkillLevel): string {
    switch (level) {
      case SkillLevel.Beginner: return 'Početnik';
      case SkillLevel.Intermediate: return 'Srednji';
      case SkillLevel.Advanced: return 'Napredni';
      default: return '';
    }
  }

  protected getSkillClass(level: SkillLevel): string {
    switch (level) {
      case SkillLevel.Beginner: return 'skill-beginner';
      case SkillLevel.Intermediate: return 'skill-intermediate';
      case SkillLevel.Advanced: return 'skill-advanced';
      default: return '';
    }
  }

  protected openAddForm() {
    this.showAddForm.set(true);
    this.selectedHobbyId.set(null);
    this.selectedSkillLevel.set(SkillLevel.Beginner);
  }

  protected cancelAdd() {
    this.showAddForm.set(false);
  }

  protected addHobby() {
    const hobbyId = this.selectedHobbyId();
    if (!hobbyId) return;

    this.isSaving.set(true);
    this.profileService.addHobby({
      hobbyId,
      skillLevel: this.selectedSkillLevel()
    }).subscribe({
      next: (hobby) => {
        this.userHobbies.set([...this.userHobbies(), hobby]);
        this.showAddForm.set(false);
        this.isSaving.set(false);
        this.notificationService.success({
          title: 'Uspjeh',
          message: 'Hobi je dodan.'
        });
      },
      error: (err) => {
        this.isSaving.set(false);
        this.notificationService.error({
          title: 'Greška',
          message: err.error?.error || 'Greška pri dodavanju hobija.'
        });
      }
    });
  }

  protected removeHobby(hobbyId: number) {
    if (!confirm('Da li ste sigurni da želite ukloniti ovaj hobi?')) return;

    this.profileService.deleteHobby(hobbyId).subscribe({
      next: () => {
        this.userHobbies.set(this.userHobbies().filter(h => h.hobbyId !== hobbyId));
        this.notificationService.success({
          title: 'Uspjeh',
          message: 'Hobi je uklonjen.'
        });
      },
      error: (err) => {
        this.notificationService.error({
          title: 'Greška',
          message: err.error?.error || 'Greška pri uklanjanju hobija.'
        });
      }
    });
  }
}
