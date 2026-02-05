import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { ProjectService } from '../../../../core/services/project.service';

interface StatCard {
  label: string;
  value: string | number;
  icon: string;
  route: string;
  color: string;
  secondaryColor: string;
  rgb: string;
  loading?: boolean;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly projectService = inject(ProjectService);

  protected readonly stats = signal<StatCard[]>([
    {
      label: 'Proizvodi',
      value: '...',
      icon: 'cube',
      route: '/admin/products',
      color: '#ff6b35',
      secondaryColor: '#f7931e',
      rgb: '255, 107, 53',
      loading: true
    },
    {
      label: 'Kategorije',
      value: '...',
      icon: 'tag',
      route: '/admin/categories',
      color: '#f7931e',
      secondaryColor: '#ff6b35',
      rgb: '247, 147, 30',
      loading: true
    },
    {
      label: 'Projekti',
      value: '...',
      icon: 'photo',
      route: '/admin/projects',
      color: '#6366f1',
      secondaryColor: '#8b5cf6',
      rgb: '99, 102, 241',
      loading: true
    },
    {
      label: 'Korisnici',
      value: '--',
      icon: 'users',
      route: '/admin/users',
      color: '#5a3a2a',
      secondaryColor: '#2c1810',
      rgb: '90, 58, 42',
      loading: false
    },
  ]);

  ngOnInit() {
    this.loadStats();
  }

  private loadStats() {
    // Load products count
    this.productService.getProducts({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(0, result.totalCount);
      },
      error: () => this.updateStat(0, 'Greška')
    });

    // Load categories count
    this.categoryService.getCategories({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(1, result.totalCount);
      },
      error: () => this.updateStat(1, 'Greška')
    });

    // Load projects count
    this.projectService.getProjects({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(2, result.totalCount);
      },
      error: () => this.updateStat(2, 'Greška')
    });
  }

  private updateStat(index: number, value: string | number) {
    const currentStats = this.stats();
    const updated = [...currentStats];
    updated[index] = { ...updated[index], value, loading: false };
    this.stats.set(updated);
  }
}
