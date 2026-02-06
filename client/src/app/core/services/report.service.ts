import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DashboardReportResponse,
  RevenueSummaryResponse,
  SalesChartDataPoint,
  TopProductResponse,
  RecentOrderResponse,
  UserGrowthResponse,
  RatingOverviewResponse
} from '../models/report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/reports`;

  getDashboardReport(): Observable<DashboardReportResponse> {
    return this.http.get<DashboardReportResponse>(`${this.baseUrl}/dashboard`);
  }

  getRevenueSummary(): Observable<RevenueSummaryResponse> {
    return this.http.get<RevenueSummaryResponse>(`${this.baseUrl}/revenue`);
  }

  getSalesChart(days: number = 30): Observable<SalesChartDataPoint[]> {
    return this.http.get<SalesChartDataPoint[]>(`${this.baseUrl}/sales-chart`, {
      params: { days: days.toString() }
    });
  }

  getTopProducts(count: number = 5): Observable<TopProductResponse[]> {
    return this.http.get<TopProductResponse[]>(`${this.baseUrl}/top-products`, {
      params: { count: count.toString() }
    });
  }

  getRecentOrders(count: number = 10): Observable<RecentOrderResponse[]> {
    return this.http.get<RecentOrderResponse[]>(`${this.baseUrl}/recent-orders`, {
      params: { count: count.toString() }
    });
  }

  getUserGrowth(days: number = 30): Observable<UserGrowthResponse> {
    return this.http.get<UserGrowthResponse>(`${this.baseUrl}/user-growth`, {
      params: { days: days.toString() }
    });
  }

  getRatingOverview(): Observable<RatingOverviewResponse> {
    return this.http.get<RatingOverviewResponse>(`${this.baseUrl}/ratings`);
  }
}
