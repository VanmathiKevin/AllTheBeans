import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  imports: [CommonModule, RouterModule]
})
export class NavbarComponent {
  constructor(private router: Router,public auth: AuthService) {}

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
  
  goToBeans() {
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/beans']);
    });
  }
}
