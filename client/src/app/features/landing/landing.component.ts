import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../core/services/product.service';
import { ProjectService } from '../../core/services/project.service';
import { AuthService } from '../../core/services/auth.service';
import { ProductResponse } from '../../core/models/product.model';
import { ProjectResponse } from '../../core/models/project.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-landing',
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly projectService = inject(ProjectService);
  private readonly authService = inject(AuthService);

  protected readonly isAuthenticated = this.authService.isAuthenticated;
  protected readonly isAdmin = this.authService.isAdmin;

  protected readonly animationComplete = signal(false);
  protected readonly showContent = signal(false);
  protected readonly isOnHeroSection = signal(true);
  protected readonly currentKitIndex = signal(0);
  protected readonly featuredKits = signal<ProductResponse[]>([]);
  protected readonly isLoadingKits = signal(true);
  protected readonly topRatedProjects = signal<ProjectResponse[]>([]);
  protected readonly isLoadingProjects = signal(true);
  protected readonly isMobileView = signal(false);

  // Computed carousel transform with adjacent items visible
  protected readonly carouselTransform = computed(() => {
    const index = this.currentKitIndex();
    if (this.isMobileView()) {
      return `translateX(-${index * 100}%)`;
    }
    // Desktop: items are 60% width with 20% offset for partial visibility
    return `translateX(calc(20% - ${index * 60}% - ${index * 2}rem))`;
  });

  private scrollListener: (() => void) | null = null;
  private resizeListener: (() => void) | null = null;
  private scrollTicking = false;
  private viewportHeight = 0;

  ngOnInit() {
    // Fetch featured products from API
    this.loadFeaturedKits();
    this.loadTopRatedProjects();

    // Start the animation sequence - faster
    setTimeout(() => {
      this.animationComplete.set(true);
      setTimeout(() => {
        this.showContent.set(true);
      }, 600);
    }, 2000);

    // Cache viewport height to avoid reflow on every scroll
    this.viewportHeight = window.innerHeight;

    // Add throttled scroll listener using requestAnimationFrame
    this.scrollListener = () => {
      if (!this.scrollTicking) {
        requestAnimationFrame(() => {
          const scrollPosition = window.scrollY;
          // User is on hero section if scroll position is less than 40% of viewport height
          this.isOnHeroSection.set(scrollPosition < this.viewportHeight * 0.4);
          this.scrollTicking = false;
        });
        this.scrollTicking = true;
      }
    };

    window.addEventListener('scroll', this.scrollListener, { passive: true });

    // Add resize listener for mobile detection and viewport height update
    this.resizeListener = () => {
      this.viewportHeight = window.innerHeight;
      this.isMobileView.set(window.innerWidth <= 768);
    };
    // Set initial value
    this.isMobileView.set(window.innerWidth <= 768);
    window.addEventListener('resize', this.resizeListener, { passive: true });
  }

  private loadFeaturedKits() {
    this.isLoadingKits.set(true);
    this.productService.getBestSelling(5).subscribe({
      next: (products) => {
        this.featuredKits.set(products);
        this.isLoadingKits.set(false);
      },
      error: (err) => {
        console.error('Failed to load best selling kits:', err);
        this.isLoadingKits.set(false);
      }
    });
  }

  private loadTopRatedProjects() {
    this.isLoadingProjects.set(true);
    this.projectService.getTopRatedProjects().subscribe({
      next: (projects) => {
        this.topRatedProjects.set(projects);
        this.isLoadingProjects.set(false);
      },
      error: (err) => {
        console.error('Failed to load top rated projects:', err);
        this.isLoadingProjects.set(false);
      }
    });
  }

  ngOnDestroy() {
    if (this.scrollListener) {
      window.removeEventListener('scroll', this.scrollListener);
    }
    if (this.resizeListener) {
      window.removeEventListener('resize', this.resizeListener);
    }
  }

  protected nextKit() {
    const len = this.featuredKits().length;
    if (len > 0) {
      this.currentKitIndex.set((this.currentKitIndex() + 1) % len);
    }
  }

  protected previousKit() {
    const len = this.featuredKits().length;
    if (len > 0) {
      this.currentKitIndex.set((this.currentKitIndex() - 1 + len) % len);
    }
  }

  protected goToKit(index: number) {
    this.currentKitIndex.set(index);
  }

  protected getImageUrl(relativePath: string | null | undefined): string | null {
    if (!relativePath) return null;
    // If it's already an absolute URL (Unsplash, etc.), return as-is
    if (relativePath.startsWith('http://') || relativePath.startsWith('https://')) {
      return relativePath;
    }
    return `${environment.baseUrl}${relativePath}`;
  }

  protected readonly embers = Array.from({ length: 15 }, (_, i) => ({
    id: i,
    delay: Math.random() * 10,
    duration: 8 + Math.random() * 6,
    left: Math.random() * 100,
    size: 4 + Math.random() * 4
  }));
}
