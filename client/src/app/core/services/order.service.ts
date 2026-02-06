import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import { OrderResponse, OrderQueryFilter, CreateOrderRequest } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/orders`;

  getOrders(filter: OrderQueryFilter): Observable<PagedResult<OrderResponse>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber?.toString() || '1')
      .set('pageSize', filter.pageSize?.toString() || '10');

    if (filter.search) params = params.set('search', filter.search);
    if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
    if (filter.userId) params = params.set('userId', filter.userId.toString());
    if (filter.status !== undefined) params = params.set('status', filter.status.toString());

    return this.http.get<PagedResult<OrderResponse>>(this.apiUrl, { params });
  }

  getOrderById(id: number): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`${this.apiUrl}/${id}`);
  }

  createTestOrder(request: CreateOrderRequest): Observable<OrderResponse> {
    return this.http.post<OrderResponse>(`${this.apiUrl}/test`, request);
  }
}
