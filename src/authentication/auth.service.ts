import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, catchError, of } from 'rxjs';

export interface UserInfo { name: string;}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  user = signal<UserInfo | null>(null);

  register(name: string, password: string) {
    return this.http.post('/api/register', { username: name, password });
  }

  login(name: string, password: string) {
    return this.http.post<UserInfo>('/api/login', { username: name, password })
      .pipe(tap(u => this.user.set(u)));
  }

  logout() {
    return this.http.post('/api/logout', {})
      .pipe(tap(() => this.user.set(null)));
  }

  // check ci ma user este platnu session po obnoveni stranky
  loadMe() {
    return this.http.get<UserInfo>('/api/me').pipe(
      tap(u => this.user.set(u)),
      catchError(() => { this.user.set(null); return of(null); })
    );
  }
}
