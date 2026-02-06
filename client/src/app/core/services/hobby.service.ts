import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HobbyResponse } from '../models/hobby.model';

@Injectable({
  providedIn: 'root'
})
export class HobbyService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/hobby`;

  getHobbies(): Observable<HobbyResponse[]> {
    return this.http.get<HobbyResponse[]>(this.apiUrl);
  }
}
