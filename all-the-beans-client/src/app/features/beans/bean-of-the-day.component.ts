import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CoffeeBean } from '../../core/models/coffee-bean.model';
import { BeanService } from '../../core/services/bean.service';

@Component({
  standalone: true,
  selector: 'app-bean-of-the-day',
  templateUrl: './bean-of-the-day.component.html',
  styleUrls: ['./bean-of-the-day.component.scss'],
  imports: [CommonModule, RouterModule]
})
export class BeanOfTheDayComponent implements OnInit {
  bean: CoffeeBean | null = null;
  error = '';
  loading = true;

  constructor(private beanService: BeanService) {}

  ngOnInit(): void {
    this.beanService.getBeanOfTheDay().subscribe({
      next: (data) => {
        this.bean = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load Bean of the Day.';
        this.loading = false;
      }
    });
  }
}
