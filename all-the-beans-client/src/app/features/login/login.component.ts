import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "../../core/services/auth.service";

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  imports: [CommonModule, FormsModule]
})
export class LoginComponent {
  
  username = '';
  password = '';
  error: string | null = null;

  constructor(private auth: AuthService, private router: Router) {}

  login() {
    this.auth.login(this.username, this.password).subscribe({
      next: res => {
        this.auth.setToken(res.token);
        this.router.navigate(['/beans']);
      },
      error: err => {
        this.error = 'Invalid username or password';
      }
    });
  }
}
