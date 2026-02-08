import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProjectReviewResponse } from '../models/project.model';
import { PagedResult } from '../models/common.model';

export interface CreateProjectReviewRequest {
  projectId: number;
  rating: number;
  comment?: string;
}

export interface UpdateProjectReviewRequest {
  rating: number;
  comment?: string;
}

@Injectable({ providedIn: 'root' })
export class ProjectReviewService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/project-reviews`;

  getByProjectId(projectId: number, pageNumber = 1, pageSize = 20): Observable<PagedResult<ProjectReviewResponse>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<ProjectReviewResponse>>(`${this.apiUrl}/project/${projectId}`, { params });
  }

  getAverageRating(projectId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/project/${projectId}/average-rating`);
  }

  hasUserReviewed(projectId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/project/${projectId}/has-reviewed`);
  }

  createReview(request: CreateProjectReviewRequest): Observable<ProjectReviewResponse> {
    return this.http.post<ProjectReviewResponse>(this.apiUrl, request);
  }

  updateReview(id: number, request: UpdateProjectReviewRequest): Observable<ProjectReviewResponse> {
    return this.http.put<ProjectReviewResponse>(`${this.apiUrl}/${id}`, request);
  }

  deleteReview(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
