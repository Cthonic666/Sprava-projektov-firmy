import {Component, inject} from '@angular/core';
import {AuthService} from '../../../authentication/auth.service';
import {Router, RouterLink} from '@angular/router';
import {FormsModule} from '@angular/forms';

@Component({
  imports: [
    FormsModule,
    RouterLink
  ],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})

export class Login {
  private auth = inject(AuthService);
  private router = inject(Router);
  name = ''; password = ''; error = '';

  submit() {
    this.auth.login(this.name, this.password).subscribe({
      next: () => this.router.navigate(['/projects']),
      error: () => this.error = 'Nesprávne meno alebo heslo',
    });
  }
}
