import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PaymentConfigResponse, PaymentIntentResponse } from '../models/payment.model';
import { CreateOrderItemRequest } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/payment`;

  getConfig(): Observable<PaymentConfigResponse> {
    return this.http.get<PaymentConfigResponse>(`${this.baseUrl}/config`);
  }

  createPaymentIntent(items: CreateOrderItemRequest[], couponCode?: string): Observable<PaymentIntentResponse> {
    return this.http.post<PaymentIntentResponse>(`${this.baseUrl}/create-intent`, { items, couponCode });
  }
}
