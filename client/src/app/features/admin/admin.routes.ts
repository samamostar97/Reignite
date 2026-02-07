import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './layout/admin-layout.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ProductListComponent } from './pages/products/product-list/product-list.component';
import { ProductFormComponent } from './pages/products/product-form/product-form.component';
import { CategoryListComponent } from './pages/categories/category-list.component';
import { SupplierListComponent } from './pages/suppliers/supplier-list.component';
import { HobbyListComponent } from './pages/hobbies/hobby-list.component';
import { ProjectListComponent } from './pages/projects/project-list.component';
import { UserListComponent } from './pages/users/user-list.component';
import { UserFormComponent } from './pages/users/user-form/user-form.component';
import { OrderListComponent } from './pages/orders/order-list.component';
import { ReviewListComponent } from './pages/reviews/review-list.component';
import { ReportsComponent } from './pages/reports/reports.component';

export const adminRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'products', component: ProductListComponent },
      { path: 'products/new', component: ProductFormComponent },
      { path: 'products/:id/edit', component: ProductFormComponent },
      { path: 'categories', component: CategoryListComponent },
      { path: 'suppliers', component: SupplierListComponent },
      { path: 'hobbies', component: HobbyListComponent },
      { path: 'projects', component: ProjectListComponent },
      { path: 'reviews', component: ReviewListComponent },
      { path: 'users', component: UserListComponent },
      { path: 'users/new', component: UserFormComponent },
      { path: 'users/:id/edit', component: UserFormComponent },
      { path: 'orders', component: OrderListComponent },
      { path: 'reports', component: ReportsComponent },
    ]
  }
];
