import { Component, Input, OnInit, ChangeDetectionStrategy } from '@angular/core';

interface EmberParticle {
  id: number;
  left: number;
  delay: number;
  duration: number;
  size: number;
}

@Component({
  selector: 'app-ember-background',
  standalone: true,
  template: `
    <div class="embers-container" [class.embers-sparse]="density === 'sparse'" [class.embers-full]="density === 'full'">
      @for (ember of embers; track ember.id) {
        <div
          class="ember"
          [style.left.%]="ember.left"
          [style.animation-delay.s]="ember.delay"
          [style.animation-duration.s]="ember.duration"
          [style.width.px]="ember.size"
          [style.height.px]="ember.size"
          [style.opacity]="maxOpacity">
        </div>
      }
    </div>
  `,
  styleUrl: './ember-background.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmberBackgroundComponent implements OnInit {
  @Input() density: 'sparse' | 'medium' | 'full' = 'medium';
  @Input() maxOpacity = 0.9;

  embers: EmberParticle[] = [];

  private get particleCount(): number {
    switch (this.density) {
      case 'sparse': return 8;
      case 'medium': return 20;
      case 'full': return 30;
    }
  }

  ngOnInit() {
    const count = window.innerWidth < 768 ? Math.max(5, Math.floor(this.particleCount / 2)) : this.particleCount;

    this.embers = Array.from({ length: count }, (_, i) => ({
      id: i,
      left: Math.random() * 100,
      delay: Math.random() * 10,
      duration: 8 + Math.random() * 6,
      size: 3 + Math.random() * 4
    }));
  }
}
