import {Component, inject, signal} from '@angular/core';
import {Router, RouterLink, RouterOutlet} from '@angular/router';
import { AuthService } from '../authentication/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app.html',
})
export class App {
  auth = inject(AuthService);
  private router = inject(Router);

  logout() {
    this.auth.logout().subscribe(() => this.router.navigate(['/login']));
  }
}
