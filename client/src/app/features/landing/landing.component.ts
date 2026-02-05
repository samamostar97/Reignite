import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../core/services/product.service';
import { ProjectService } from '../../core/services/project.service';
import { Product } from '../../core/models/product.model';
import { Project } from '../../core/models/project.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-landing',
  imports: [CommonModule],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly projectService = inject(ProjectService);

  protected readonly animationComplete = signal(false);
  protected readonly showContent = signal(false);
  protected readonly isOnHeroSection = signal(true);
  protected readonly currentKitIndex = signal(0);
  protected readonly featuredKits = signal<Product[]>([]);
  protected readonly isLoadingKits = signal(true);
  protected readonly topRatedProjects = signal<Project[]>([]);
  protected readonly isLoadingProjects = signal(true);
  protected readonly isMobileView = signal(false);

  // Computed carousel transform that responds to mobile/desktop
  protected readonly carouselTransform = computed(() => {
    const index = this.currentKitIndex();
    if (this.isMobileView()) {
      // Mobile: items are 100% width, simple slide (no gap on mobile)
      return `translateX(-${index * 100}%)`;
    }
    // Desktop: items are 60% width with 20% offset for partial visibility
    return `translateX(calc(20% - ${index * 60}% - ${index * 2}rem))`;
  });

  private scrollListener: (() => void) | null = null;
  private kitCarouselInterval: any = null;
  private resizeListener: (() => void) | null = null;

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

    // Add scroll listener to detect when user scrolls past hero section
    this.scrollListener = () => {
      const scrollPosition = window.scrollY;
      const viewportHeight = window.innerHeight;

      // User is on hero section if scroll position is less than 40% of viewport height
      this.isOnHeroSection.set(scrollPosition < viewportHeight * 0.4);
    };

    window.addEventListener('scroll', this.scrollListener, { passive: true });

    // Add resize listener for mobile detection
    this.resizeListener = () => {
      const wasMobile = this.isMobileView();
      const isMobile = window.innerWidth <= 768;
      this.isMobileView.set(isMobile);

      // Stop carousel when switching to mobile, start when switching to desktop
      if (isMobile && !wasMobile) {
        this.stopKitCarousel();
      } else if (!isMobile && wasMobile) {
        this.startKitCarousel();
      }
    };
    // Set initial value
    this.isMobileView.set(window.innerWidth <= 768);
    window.addEventListener('resize', this.resizeListener, { passive: true });

    // Start kit carousel auto-rotation (only on desktop)
    this.startKitCarousel();
  }

  private loadFeaturedKits() {
    this.isLoadingKits.set(true);
    this.productService.getProducts({ pageSize: 6 }).subscribe({
      next: (result) => {
        this.featuredKits.set(result.items);
        this.isLoadingKits.set(false);
      },
      error: (err) => {
        console.error('Failed to load featured kits:', err);
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
    if (this.kitCarouselInterval) {
      clearInterval(this.kitCarouselInterval);
    }
  }

  private startKitCarousel() {
    // Don't auto-rotate on mobile
    if (this.isMobileView()) {
      return;
    }
    this.stopKitCarousel();
    this.kitCarouselInterval = setInterval(() => {
      this.nextKit();
    }, 3500);
  }

  private stopKitCarousel() {
    if (this.kitCarouselInterval) {
      clearInterval(this.kitCarouselInterval);
      this.kitCarouselInterval = null;
    }
  }

  private restartKitCarousel() {
    this.stopKitCarousel();
    setTimeout(() => {
      this.startKitCarousel();
    }, 5000);
  }

  // Simple infinite loop: 0 -> 1 -> 2 -> 0 -> 1 -> 2 ...
  protected nextKit(isManual = false) {
    if (isManual) {
      this.restartKitCarousel();
    }
    const len = this.featuredKits().length;
    if (len > 0) {
      this.currentKitIndex.set((this.currentKitIndex() + 1) % len);
    }
  }

  protected previousKit() {
    this.restartKitCarousel();
    const len = this.featuredKits().length;
    if (len > 0) {
      this.currentKitIndex.set((this.currentKitIndex() - 1 + len) % len);
    }
  }

  protected goToKit(index: number) {
    this.restartKitCarousel();
    this.currentKitIndex.set(index);
  }

  protected getImageUrl(relativePath: string | null | undefined): string | null {
    if (!relativePath) return null;
    return `${environment.baseUrl}${relativePath}`;
  }

  protected readonly embers = Array.from({ length: 40 }, (_, i) => ({
    id: i,
    delay: Math.random() * 10,
    duration: 6 + Math.random() * 8,
    left: Math.random() * 100,
    size: 5 + Math.random() * 6
  }));
}
