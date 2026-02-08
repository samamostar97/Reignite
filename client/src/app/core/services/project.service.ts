import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import { ProjectResponse, ProjectQueryFilter, CreateProjectRequest } from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/projects`;

  getProjects(filter?: ProjectQueryFilter): Observable<PagedResult<ProjectResponse>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
      if (filter.hobbyId) params = params.set('hobbyId', filter.hobbyId.toString());
    }

    return this.http.get<PagedResult<ProjectResponse>>(this.apiUrl, { params });
  }

  getTopRatedProjects(pageNumber: number = 1, pageSize: number = 3): Observable<PagedResult<ProjectResponse>> {
    return this.http.get<PagedResult<ProjectResponse>>(`${this.apiUrl}/top-rated`, {
      params: { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() }
    });
  }

  getProjectById(id: number): Observable<ProjectResponse> {
    return this.http.get<ProjectResponse>(`${this.apiUrl}/${id}`);
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  createProject(request: CreateProjectRequest): Observable<ProjectResponse> {
    return this.http.post<ProjectResponse>(this.apiUrl, request);
  }

  uploadProjectImage(id: number, file: File): Observable<{ fileUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ fileUrl: string }>(`${environment.apiUrl}/uploads/projects/${id}`, formData);
  }

  deleteProjectImage(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/uploads/projects/${id}`);
  }
}
