import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../core/models/product.model';

interface FeaturedProject {
  id: number;
  title: string;
  author: string;
  description: string;
  image: string;
  likes: number;
}

@Component({
  selector: 'app-landing',
  imports: [CommonModule],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);

  protected readonly animationComplete = signal(false);
  protected readonly showContent = signal(false);
  protected readonly isOnHeroSection = signal(true);
  protected readonly currentKitIndex = signal(0);
  protected readonly featuredKits = signal<Product[]>([]);
  protected readonly isLoadingKits = signal(true);

  private scrollListener: (() => void) | null = null;
  private kitCarouselInterval: any = null;

  ngOnInit() {
    // Fetch featured products from API
    this.loadFeaturedKits();

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

    // Start kit carousel auto-rotation
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

  ngOnDestroy() {
    if (this.scrollListener) {
      window.removeEventListener('scroll', this.scrollListener);
    }
    if (this.kitCarouselInterval) {
      clearInterval(this.kitCarouselInterval);
    }
  }

  private startKitCarousel() {
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

  protected readonly embers = Array.from({ length: 40 }, (_, i) => ({
    id: i,
    delay: Math.random() * 10,
    duration: 6 + Math.random() * 8,
    left: Math.random() * 100,
    size: 5 + Math.random() * 6
  }));

  protected readonly featuredProjects: FeaturedProject[] = [
    {
      id: 1,
      title: 'Ručno Tkana Torba',
      author: 'Amina K.',
      description: 'Tradicionalna tehnika tkanja sa modernim dizajnom. Trajalo mi je tri sedmice, ali vrijedi svaki trenutak.',
      image: 'https://images.unsplash.com/photo-1590874103328-eac38a683ce7?w=400&h=300&fit=crop',
      likes: 147
    },
    {
      id: 2,
      title: 'Keramički Servis',
      author: 'Emir D.',
      description: 'Set od 6 šoljica i tanjira. Svaki komad je jedinstven, upravo kako sam i htio.',
      image: 'https://images.unsplash.com/photo-1610701596007-11502861dcfa?w=400&h=300&fit=crop',
      likes: 203
    },
    {
      id: 3,
      title: 'Zidna Dekoracija',
      author: 'Sara M.',
      description: 'Makrame kombinacija sa sušenim cvijećem. Savršeno se uklapa u moj dnevni boravak.',
      image: 'https://images.unsplash.com/photo-1616486029423-aaa4789e8c9a?w=400&h=300&fit=crop',
      likes: 189
    }
  ];
}
