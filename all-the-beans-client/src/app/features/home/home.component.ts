import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BeanOfTheDayComponent } from '../beans/bean-of-the-day.component';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule, BeanOfTheDayComponent, RouterModule]
})
export class HomeComponent {
  constructor(public auth: AuthService, private router: Router) {}

  navigate() {
    if (this.auth.isLoggedIn()) {
      this.router.navigate(['/beans']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}
