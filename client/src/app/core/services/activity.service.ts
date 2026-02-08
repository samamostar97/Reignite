import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ActivityResponse, ActivityQueryFilter } from '../models/activity.model';
import { PagedResult } from '../models/common.model';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/activities`;

  getActivities(filter: ActivityQueryFilter): Observable<PagedResult<ActivityResponse>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber?.toString() || '1')
      .set('pageSize', filter.pageSize?.toString() || '10');

    if (filter.type !== undefined) {
      params = params.set('type', filter.type.toString());
    }

    return this.http.get<PagedResult<ActivityResponse>>(this.apiUrl, { params });
  }
}
