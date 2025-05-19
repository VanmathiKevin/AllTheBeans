import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BeanOfTheDayComponent } from '../beans/bean-of-the-day.component';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule, BeanOfTheDayComponent, RouterModule]
})
export class HomeComponent {}
