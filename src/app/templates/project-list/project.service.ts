import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface Project { id: string; name: string; abbreviation: string ; customer: string }
export type ProjectDto = Omit<Project, 'id'>;

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private http = inject(HttpClient);
  private url = '/api/projects';

  getAll()                          { return this.http.get<Project[]>(this.url); }
  create(dto: ProjectDto)          { return this.http.post<Project>(this.url, dto); }
  update(id: string, dto: ProjectDto) { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: string)                { return this.http.delete<void>(`${this.url}/${id}`); }
}
