import {Component, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {AuthService} from '../../../authentication/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
})
export class Register {
  private auth = inject(AuthService);
  private router = inject(Router);

  name = '';
  password = '';
  password2 = '';
  error = '';
  success = false;

  submit() {
    this.error = '';
    /*
        if (this.password.length < 6) {
          this.error = 'Heslo musí mať aspoň 6 znakov.';
          return;
        }
    */
        if (this.password !== this.password2) {
          this.error = 'Heslá sa nezhodujú.';
          return;
        }

    this.auth.register(this.name, this.password).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: err => {
        this.error = err.status === 409
          ? 'Používateľ s týmto menom už existuje.'
          : 'Registrácia zlyhala.';
      },
    });
  }
}
