import { Component, OnInit } from '@angular/core';
import { CoffeeBean } from '../../../core/models/coffee-bean.model';
import { BeanService } from '../../../core/services/bean.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-coffee-bean-list',
  templateUrl: './coffee-bean-list.component.html',
  styleUrls: ['./coffee-bean-list.component.scss'],
  imports: [CommonModule, RouterModule]
})
export class CoffeeBeanListComponent implements OnInit {
  beans: CoffeeBean[] = [];
  loading = true;
  error: string | null = null;

  constructor(private beanService: BeanService) {}

  ngOnInit(): void {
    this.beanService.getAll().subscribe({
      next: (data) => {
        this.beans = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load coffee beans.';
        this.loading = false;
      }
    });
  }
}
